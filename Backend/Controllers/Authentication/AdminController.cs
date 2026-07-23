using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;
using TravelWithCode.Request;
using Npgsql;

namespace TravelWithCode.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class AdminController : ControllerBase
{
    private readonly Postgresql _postgresql;

    public AdminController(Postgresql postgresql)
    {
        _postgresql = postgresql;
    }

    [HttpGet("api/admin/check")]
    public async Task<IActionResult> Admin()
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
                        return Ok(true);
                    } else {
                        return Ok(false);
                    }
                }
            }
        }

        return BadRequest("Very big problem.");
    }
}