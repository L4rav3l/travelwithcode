using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;
using TravelWithCode.Request;
using Npgsql;

namespace TravelWithCode.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]
public class StatusLxcController : ControllerBase
{

    private readonly Postgresql _postgresql;

    public StatusLxcController(Postgresql postgresql)
    {
        _postgresql = postgresql;
    }

    [HttpGet("api/proxmox/lxc")]
    public async Task<IActionResult> Status()
    {
        var userID = HttpContext.Items["UserId"];

        await using(var conn = await _postgresql.GetOpenConnectionAsync())
        {
            await using(var userData = new NpgsqlCommand("SELECT * FROM users WHERE id = @id", conn))
            {
                userData.Parameters.AddWithValue("id", Convert.ToInt32(userID));
            
                await using(var reader = await userData.ExecuteReaderAsync())
                {
                    if(await reader.ReadAsync())
                    {
                        int lxcId = reader.GetInt32(reader.GetOrdinal("lxcid"));
                        return Ok(lxcId);
                    }
                }
            }
        }

        return BadRequest();
    }

}