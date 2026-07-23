using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;
using TravelWithCode.Request;
using Npgsql;

namespace TravelWithCode.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class AdminStatusController : ControllerBase
{
    private readonly Postgresql _postgresql;

    public AdminStatusController(Postgresql postgresql)
    {
        _postgresql = postgresql;
    }

    [HttpGet("api/admin/status")]
    public async Task<IActionResult> AdminStatus([FromQuery] AdminStatusRequest request)
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

        await using(var conn = await _postgresql.GetOpenConnectionAsync())
        {
            await using(var adminStatus = new NpgsqlCommand("SELECT * FROM users WHERE id = @id", conn))
            {
                adminStatus.Parameters.AddWithValue("id", request.Id);

                await using(var reader = await adminStatus.ExecuteReaderAsync())
                {
                    if(await reader.ReadAsync())
                    {
                        return Ok(reader.GetBoolean(reader.GetOrdinal("admin")));
                    }
                }
            }
        }

        return BadRequest();
    }
}