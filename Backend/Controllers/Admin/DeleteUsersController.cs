using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;
using TravelWithCode.Request;
using Npgsql;

namespace TravelWithCode.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class DeleteUsersController : ControllerBase
{
    private readonly Postgresql _postgresql;

    public DeleteUsersController(Postgresql postgresql)
    {
        _postgresql = postgresql;
    }

    [HttpPost("api/admin/delete_user")]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUsersRequest request)
    {
        var userID = HttpContext.Items["UserId"];
        int id = request.Id;

        if(Convert.ToInt32(userID) == request.Id)
        {
            return Conflict("You cannot delete yourself as an admin."); 
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

            await using(var deleteUser = new NpgsqlCommand("DELETE FROM users WHERE id = @id", conn))
            {
                deleteUser.Parameters.AddWithValue("id", id);

                await deleteUser.ExecuteNonQueryAsync();
            }
        }

        return Ok();
    }
}