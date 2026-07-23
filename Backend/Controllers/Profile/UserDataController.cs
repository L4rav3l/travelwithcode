using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;
using Npgsql;

namespace TravelWithCode.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class UserDataController : ControllerBase
{
    private readonly Postgresql _postgresql;

    public UserDataController(Postgresql postgresql)
    {
        _postgresql = postgresql;
    }

    [HttpGet("api/profile/data")]
    public async Task<IActionResult> Data()
    {
        string username = "";
        var userID = HttpContext.Items["UserId"];

        await using(var conn = await _postgresql.GetOpenConnectionAsync())
        {
            await using(var data = new NpgsqlCommand("SELECT * FROM users WHERE id = @id", conn))
            {
                data.Parameters.AddWithValue("id", userID);

                await using(var reader = await data.ExecuteReaderAsync())
                {
                    if(await reader.ReadAsync())
                    {
                        username = reader.GetString(reader.GetOrdinal("username"));
                    }
                }
            }
        }

        return Ok(username);
    }
}