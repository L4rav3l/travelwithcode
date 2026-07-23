using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;
using TravelWithCode.Request;
using Npgsql;

namespace TravelWithCode.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class DeleteLxcContainer : ControllerBase
{
    private readonly Postgresql _postgresql;
    private readonly ProxmoxService _proxmoxService;
    private readonly SSHService _sshService;
    
    public DeleteLxcContainer(Postgresql postgresql, ProxmoxService proxmoxService, SSHService sshService)
    {
        _postgresql = postgresql;
        _proxmoxService = proxmoxService;
        _sshService = sshService;
    }

    [HttpPost("api/proxmox/delete_lxc")]
    public async Task<IActionResult> DeleteLxc(DeleteLxcRequest request)
    {
        int lxcId = request.LxcId;

        using(var client = await _sshService.CreateSSHConnection($"192.168.122.{lxcId}", "root", "03Q@bD96GzWv"))
        {
            var command1 = client.CreateCommand("cd *");
        }

        return Ok();
    }
}