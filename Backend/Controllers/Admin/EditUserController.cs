using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;
using TravelWithCode.Request;
using Npgsql;

namespace TravelWithCode.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class EditUserController : ControllerBase
{
    private readonly Postgresql _postgresql;
    private readonly Argon2 _argon2;

    public EditUserController(Postgresql postgresql)
    {
        _postgresql = postgresql;
    }

    [HttpPost("api/admin/edit_user")]
    public async Task<IActionResult> EditUser([FromBody] EditUserRequest request)
    {
        var userID = HttpContext.Items["UserId"];

        if(Convert.ToInt32(userID) == request.Id)
        {
            return Conflict("You cannot edit yourself as an admin."); 
        }

        await using(var conn = await _postgresql.GetOpenConnectionAsync())
        {
            await using(var checkAdmin = new NpgsqlCommand("SELECT * FROM users WHERE admin = true AND id = @id", conn))
            {
                checkAdmin.Parameters.AddWithValue("id", Convert.ToInt32(userID));

                await using(var reader = await checkAdmin.ExecuteReaderAsync())
                {
                    if(await reader.ReadAsync())
                    {

                    } else {
                        return Unauthorized("You are not Admin!");
                    }
                }
            }
        }

        int id = request.Id;
        string username = request.Username;
        bool isAdmin = request.Admin;

        string password = request.Password;
        
        string salt = _argon2.GenerateSalt();
        string encryptedPassword = _argon2.HashPassword(password, salt);

        await using(var conn = await _postgresql.GetOpenConnectionAsync())
        {
            await using(var checkUsername = new NpgsqlCommand("SELECT id FROM users WHERE username = @username", conn))
            {
                checkUsername.Parameters.AddWithValue("username", username);

                await using(var reader = await checkUsername.ExecuteReaderAsync())
                {
                    if(await reader.ReadAsync())
                    {
                        return Conflict("This username already exists.");
                    }
                }
            }

            await using(var updateUsers = new NpgsqlCommand("UPDATE users SET username = @username, password = @password, salt = @salt, admin = @admin WHERE id = @id", conn))
            {
                updateUsers.Parameters.AddWithValue("username", username);
                updateUsers.Parameters.AddWithValue("password", encryptedPassword);
                updateUsers.Parameters.AddWithValue("salt", salt);
                updateUsers.Parameters.AddWithValue("admin", isAdmin);

                await updateUsers.ExecuteNonQueryAsync();
            }
        }

        return Ok();
    }
}