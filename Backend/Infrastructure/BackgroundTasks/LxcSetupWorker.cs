using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TravelWithCode.Infrastructure;
using Renci.SshNet;

namespace TravelWithCode.Infrastructure;

public class LxcSetupWorker : BackgroundService
{
    private readonly ILxcTaskQueue _taskQueue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LxcSetupWorker> _logger;

    public LxcSetupWorker(ILxcTaskQueue taskQueue, IServiceProvider serviceProvider, ILogger<LxcSetupWorker> logger)
    {
        _taskQueue = taskQueue;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("LxcSetupWorker started...");

        while(!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var task = await _taskQueue.DequeueAsync(cancellationToken);

                using(var scope = _serviceProvider.CreateScope())
                {
                    var proxmoxService = scope.ServiceProvider.GetRequiredService<ProxmoxService>();
                    var sshService = scope.ServiceProvider.GetRequiredService<SSHService>();

                    await proxmoxService.CreateLXCAsync(task.containerId);
                    await proxmoxService.StartLXCAsync(task.containerId);

                    await Task.Delay(20000);

                    Console.WriteLine("Done");

                    using(var client = await sshService.CreateSSHConnection($"192.168.122.{task.containerId}", "root", "03Q@bD96GzWv"))
                    { 
                        var command = client.CreateCommand("apt install git nvim -y");
                        command.Execute();

                        var command2 = client.CreateCommand($"git clone https://token:{task.Token}@github.com/{task.github}");
                        command2.Execute();

                        var command3 = client.CreateCommand("cd *");
                        command3.Execute();
                    }
                }
            }
            catch(OperationCanceledException)
            {
                break;
            }
            catch(Exception ex)
            {
                _logger.LogError($"{ex}");
            }
        }
    }
}