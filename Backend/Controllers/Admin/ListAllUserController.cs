using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;
using TravelWithCode.Request;
using Npgsql;

namespace TravelWithCode.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class ListAllUserController : ControllerBase
{
    private readonly Postgresql _postgresql;

    public ListAllUserController(Postgresql postgresql)
    {
        _postgresql = postgresql;
    }

    [HttpGet("api/admin/list")]
    public async Task<IActionResult> ListAllUser()
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

        List<ListAllUserRequest> listofUser = new List<ListAllUserRequest>();

        await using(var conn = await _postgresql.GetOpenConnectionAsync())
        {
            await using(var ListAllUser = new NpgsqlCommand("SELECT * FROM users", conn))
            {
                await using(var reader = await ListAllUser.ExecuteReaderAsync())
                {
                    while(await reader.ReadAsync())
                    {
                        var user = new ListAllUserRequest{
                            Username = reader.GetString(reader.GetOrdinal("username")),
                            Admin = reader.GetBoolean(reader.GetOrdinal("admin")),
                            Id = reader.GetInt32(reader.GetOrdinal("id"))
                        };

                        listofUser.Add(user);
                    }
                }
            }
        }

        return Ok(listofUser);
    }
}