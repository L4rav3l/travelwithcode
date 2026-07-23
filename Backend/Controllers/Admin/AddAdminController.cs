using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;
using TravelWithCode.Request;
using Npgsql;

namespace TravelWithCode.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class AddAdminController : ControllerBase
{
    private readonly Postgresql _postgresql;

    public AddAdminController(Postgresql postgresql)
    {
        _postgresql = postgresql;
    }

    [HttpPost("api/admin/add_admin")]
    public async Task<IActionResult> AddAdmin([FromBody] AddAdminRequest request)
    {
        var userID = HttpContext.Items["UserId"];
        int id = request.Id;

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

            await using(var updateAdmin = new NpgsqlCommand("UPDATE users SET admin = true WHERE id = @id", conn))
            {
                updateAdmin.Parameters.AddWithValue("id", id);

                await updateAdmin.ExecuteNonQueryAsync();
            }
        }

        return Ok();

    }
}