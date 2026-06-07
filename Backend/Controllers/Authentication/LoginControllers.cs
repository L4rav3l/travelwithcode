using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;
using TravelWithCode.Request;
using Npgsql;

namespace TravelWithCode.Controllers;

[ApiController]
public class LoginController : ControllerBase
{
    private readonly Argon2 _argon2;
    private readonly JsonWebToken _jsonwebtoken;
    private readonly Postgresql _postgresql;

    public LoginController(Argon2 argon2, JsonWebToken jsonwebtoken, Postgresql postgresql)
    {
        _argon2 = argon2;
        _jsonwebtoken = jsonwebtoken;
        _postgresql = postgresql;
    }

    [HttpPost("api/auth/login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        string username = request.Username;
        string password = request.Password;

        await using(var conn = await _postgresql.GetOpenConnectionAsync())
        {
            await using(var userData = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", conn))
            {
                userData.Parameters.AddWithValue("username", username);

                await using(var reader = await userData.ExecuteReaderAsync())
                {

                    if(await reader.ReadAsync())
                    {
                        string encryptedPassword = reader.GetString(reader.GetOrdinal("password"));
                        string salt = reader.GetString(reader.GetOrdinal("salt"));

                        if(encryptedPassword == _argon2.HashPassword(password, salt))
                        {
                            string token = _jsonwebtoken.GenerateToken(reader.GetInt32(reader.GetOrdinal("id")), reader.GetInt32(reader.GetOrdinal("token_version")));
                                
                            return Ok(new {status = 1, token = token});
                        } else {
                            return Unauthorized(new { error = "Invalid credentials." });
                        }

                    } else {
                        return Unauthorized(new { error = "Invalid credentials." });
                    }
                }
            }
        }

        return BadRequest();
    }
}
