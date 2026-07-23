using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;
using TravelWithCode.Request;
using Npgsql;

namespace TravelWithCode.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]
public class ChangePasswordController : ControllerBase
{
    private readonly Postgresql _postgresql;
    private readonly Argon2 _argon2;
    
    public ChangePasswordController(Postgresql postgresql, Argon2 argon2)
    {
        _argon2 = argon2;
        _postgresql = postgresql;
    }

[HttpPost("api/profile/change_password")]
public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
{
    int parsedUserId = Convert.ToInt32(HttpContext.Items["UserId"]);
    string oldPassword = request.OldPassword;
    string newPassword = request.NewPassword;

    string salt1 = _argon2.GenerateSalt();

    await using(var conn = await _postgresql.GetOpenConnectionAsync())
    {
        await using(var transaction = await conn.BeginTransactionAsync())
        {
            try
            {
                string passwordDatabase = string.Empty;
                string saltDatabase = string.Empty;
                bool userExists = false;

                await using(var passwordMatch = new NpgsqlCommand("SELECT password, salt FROM users WHERE id = @userid", conn, transaction))
                {
                    passwordMatch.Parameters.AddWithValue("userid", parsedUserId);

                    await using(var reader = await passwordMatch.ExecuteReaderAsync())
                    {
                        if(await reader.ReadAsync())
                        {
                            userExists = true;
                            passwordDatabase = reader.GetString(reader.GetOrdinal("password"));
                            saltDatabase = reader.GetString(reader.GetOrdinal("salt"));
                        }
                    }
                }

                if (!userExists)
                {
                    return NotFound("");
                }

                string oldPasswordWithSalt = _argon2.HashPassword(oldPassword, saltDatabase);
                
                if (oldPasswordWithSalt != passwordDatabase)
                {
                    return Unauthorized("");
                }

                string salt = _argon2.GenerateSalt();
                string encryptedPassword = _argon2.HashPassword(newPassword, salt);

                await using(var passwordUpdate = new NpgsqlCommand("UPDATE users SET password = @password, salt = @salt, token_version = token_version + 1 WHERE id = @userid", conn, transaction))
                {
                    passwordUpdate.Parameters.AddWithValue("password", encryptedPassword);
                    passwordUpdate.Parameters.AddWithValue("salt", salt);
                    passwordUpdate.Parameters.AddWithValue("userid", parsedUserId);

                    await passwordUpdate.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                await transaction.RollbackAsync();
                return BadRequest();
            }
        }
    }

    return Ok("");
}
}