using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;
using TravelWithCode.Request;
using Npgsql;

namespace TravelWithCode.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]
public class CreateLxcController : ControllerBase
{
    private readonly Postgresql _postgresql;
    private readonly Ciper _ciper;
    private readonly ProxmoxService _proxmoxService;
    private readonly SSHService _sshService;
    private readonly ILxcTaskQueue _taskQueue;

    public CreateLxcController(Postgresql postgresql, Ciper ciper, ProxmoxService proxmoxService, SSHService sshService, ILxcTaskQueue taskQueue)
    {
        _postgresql = postgresql;
        _ciper = ciper;
        _proxmoxService = proxmoxService;
        _sshService = sshService;
        _taskQueue = taskQueue;
    }

    [HttpPost("api/proxmox/create_lxc")]
    public async Task<IActionResult> CreateLxc([FromBody] CreateLxcRequest request)
    {
        var userID = HttpContext.Items["UserId"];
        var tokenVersion = HttpContext.Items["TokenVersion"];

        string githubRepo = request.githubRepo;
        string token = "";

        NpgsqlConnection conn = await _postgresql.GetOpenConnectionAsync();
        NpgsqlTransaction transaction = null;

        await using(var userData = new NpgsqlCommand("SELECT * FROM users WHERE id = @id AND token_version = @tokenVersion AND lxcId = 0", conn))
        {
            userData.Parameters.AddWithValue("id", userID);
            userData.Parameters.AddWithValue("tokenVersion", tokenVersion);

            await using(var reader = await userData.ExecuteReaderAsync())
            {
                if(await reader.ReadAsync())
                {
                    token = _ciper.Decrypt(reader.GetString(reader.GetOrdinal("github_token")), reader.GetString(reader.GetOrdinal("github_embedding")));
                } else {
                    return NotFound();
                }
            }
        }

        try
        {
            transaction = await conn.BeginTransactionAsync();

            int lxcId = await _proxmoxService.NextIdAsync();
            
            await using(var updateUser = new NpgsqlCommand("UPDATE users SET lxcId = @lxcId WHERE token_version = @tokenVersion AND lxcId = 0 RETURNING id", conn, transaction))
            {
                updateUser.Parameters.AddWithValue("lxcId", lxcId);
                updateUser.Parameters.AddWithValue("id", userID);
                updateUser.Parameters.AddWithValue("tokenVersion", tokenVersion);

                await using(var reader = await updateUser.ExecuteReaderAsync())
                {
                    if(await reader.ReadAsync())
                    {

                    } else {
                        return Conflict();
                    }
                }
            }

            _taskQueue.QueueLxcSetup(lxcId, token, githubRepo);

            await transaction.CommitAsync();
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
            await transaction.RollbackAsync();
        }

        return Ok();
    }
}