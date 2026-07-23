using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;
using TravelWithCode.Request;
using Npgsql;

namespace TravelWithCode.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class CreateUsersController : ControllerBase
{
    private readonly Postgresql _postgresql;
    private readonly Argon2 _argon2;

    public CreateUsersController(Postgresql postgresql, Argon2 argon2)
    {
        _postgresql = postgresql;
        _argon2 = argon2;
    }

    [HttpPost("api/admin/create_user")]
    public async Task<IActionResult> CreateUsers([FromBody] CreateUsersRequest request)
    {
        var userID = HttpContext.Items["UserId"];

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

        string username = request.Username;
        string password = request.Password;
        bool admin = request.Admin;

        string salt = _argon2.GenerateSalt();
        string encryptedPassword = _argon2.HashPassword(password, salt);

        await using(var conn = await _postgresql.GetOpenConnectionAsync())
        {
            await using(var insertUser = new NpgsqlCommand("INSERT INTO users (username, password, salt, admin) VALUES (@username, @password, @salt, @admin)"))
            {
                insertUser.Parameters.AddWithValue("username");
                insertUser.Parameters.AddWithValue("password", encryptedPassword);
                insertUser.Parameters.AddWithValue("salt", salt);
                insertUser.Parameters.AddWithValue("admin", admin);

                await insertUser.ExecuteNonQueryAsync();
            }
        }

        return Ok();
    }
}