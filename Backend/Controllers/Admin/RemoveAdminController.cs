using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;
using TravelWithCode.Request;
using Npgsql;

namespace TravelWithCode.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]
public class RemoveAdminController : ControllerBase
{
    private readonly Postgresql _postgresql;
    
    public RemoveAdminController(Postgresql postgresql)
    {
        _postgresql = postgresql;
    }

    [HttpPost("api/admin/remove_admin")]
    public async Task<IActionResult> RemoveAdmin([FromBody] RemoveAdminRequest request)
    {
        var userID = HttpContext.Items["UserId"];

        if(Convert.ToInt32(userID) == request.Id)
        {
            return Conflict("You cannot remove yourself as an admin.");
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

        await using(var conn = await _postgresql.GetOpenConnectionAsync())
        {
            await using(var updateUsers = new NpgsqlCommand("UPDATE users SET admin = false WHERE id = @id", conn))
            {
                updateUsers.Parameters.AddWithValue("id", id);

                await updateUsers.ExecuteNonQueryAsync();
            }
        }

        return Ok();
    }
}