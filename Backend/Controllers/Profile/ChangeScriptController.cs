using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;
using TravelWithCode.Request;
using Npgsql;

namespace TravelWithCode.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class ChangeScriptController : ControllerBase
{
    private readonly Postgresql _postgresql;

    public ChangeScriptController(Postgresql postgresql)
    {
        _postgresql = postgresql;
    }
    
    [HttpPost("api/profile/change_script")]
    public async Task<IActionResult> ChangeScript([FromBody] ChangeScriptRequest request)
    {
        string script = request.Script;

        var userID = HttpContext.Items["UserId"];

        await using(var conn = await _postgresql.GetOpenConnectionAsync())
        {
            await using(var updateScript = new NpgsqlCommand("UPDATE users SET script = @script WHERE id = @userid", conn))
            {
                updateScript.Parameters.AddWithValue("script", script);
                updateScript.Parameters.AddWithValue("userid", Convert.ToInt32(userID));

                await updateScript.ExecuteNonQueryAsync();
            }
        }

        return Ok();
    }

    [HttpGet("api/profile/script")]
    public async Task<IActionResult> GetScript()
    {
        string script = "";

        var userID = HttpContext.Items["UserId"];

        await using(var conn = await _postgresql.GetOpenConnectionAsync())
        {
            await using(var readScript = new NpgsqlCommand("SELECT script FROM users WHERE id = @userid", conn))
            {
                readScript.Parameters.AddWithValue("userid", userID);

                await using(var reader = await readScript.ExecuteReaderAsync())
                {
                    if(await reader.ReadAsync())
                    {
                        script = reader.GetString(reader.GetOrdinal("script"));
                    }
                }
            }
        }

        return Ok(script);
    }
}