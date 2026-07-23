using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;
using TravelWithCode.Request;
using Npgsql;

namespace TravelWithCode.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class ChangeUsernameController : ControllerBase
{
    private readonly Postgresql _postgresql;
    private readonly Argon2 _argon2;

    public ChangeUsernameController(Postgresql postgresql, Argon2 argon2)
    {
        _postgresql = postgresql;
        _argon2 = argon2;
    }

    [HttpPost("api/profile/change_username")]
    public async Task<IActionResult> ChangeUsername([FromBody] ChangeUsernameRequest request)
    {
        string username = request.Username;
        string password = request.Password;
        
        var userID = HttpContext.Items["UserId"];

        await using(var conn = await _postgresql.GetOpenConnectionAsync())
        {
            await using(var transaction = await conn.BeginTransactionAsync())
            {
                try
                {
                    await using(var checkPassword = new NpgsqlCommand("SELECT * FROM users WHERE id = @id", conn, transaction))
                    {
                        checkPassword.Parameters.AddWithValue("id", Convert.ToInt32(userID));

                        await using(var reader = await checkPassword.ExecuteReaderAsync())
                        {
                            if(await reader.ReadAsync())
                            {
                                string salt = reader.GetString(reader.GetOrdinal("salt"));
                                string databasePassword = reader.GetString(reader.GetOrdinal("password"));

                                string userPassword = _argon2.HashPassword(password, salt);

                                if(userPassword != databasePassword)
                                {
                                    await transaction.RollbackAsync();
                                    return Unauthorized();
                                }
                            }
                        }
                    }

                    await using(var checkUsername = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", conn, transaction))
                    {
                        checkUsername.Parameters.AddWithValue("username", username);

                        await using(var reader = await checkUsername.ExecuteReaderAsync())
                        {
                            if(await reader.ReadAsync())
                            {
                                return Conflict();
                            }
                        }
                    }

                    await using(var updateUsername = new NpgsqlCommand("UPDATE users SET username = @username WHERE id = @id", conn, transaction))
                    {
                        updateUsername.Parameters.AddWithValue("username", username);
                        updateUsername.Parameters.AddWithValue("id", Convert.ToInt32(userID));

                        await updateUsername.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return BadRequest();
                }
            }
        }
    
        return Ok();
    }
}