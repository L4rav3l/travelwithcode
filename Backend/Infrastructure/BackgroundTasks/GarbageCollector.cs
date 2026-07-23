using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TravelWithCode.Infrastructure;
using Npgsql;

namespace TravelWithCode.Infrastructure;

public class GarbageCollector : BackgroundService
{
    private readonly Postgresql _postgresql;
    private readonly ProxmoxService _proxmoxService;

    private readonly ILogger<GarbageCollector> _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly TimeSpan _interval = TimeSpan.FromMinutes(15);

    public GarbageCollector(Postgresql postgresql, ProxmoxService proxmoxService, ILogger<GarbageCollector> logger)
    {
        _postgresql = postgresql;
        _proxmoxService = proxmoxService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(_interval);

        while(await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DoGarbageCollectionAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError($"ERROR over garbage collection: {ex}");
            }
        }
    }

    private async Task DoGarbageCollectionAsync()
    {

        _logger.LogInformation("Garbage collecting started...");

        Dictionary<int, bool> ListOfLxc = new Dictionary<int, bool>();

        for(int i = 100; i < 256; i++)
        {
            ListOfLxc[i] = false;
        }

        await using(var conn = await _postgresql.GetOpenConnectionAsync())
        {
            await using(var garbage = new NpgsqlCommand("SELECT lxcid FROM users WHERE lxcid != 0", conn))
            {
                await using(var reader = await garbage.ExecuteReaderAsync())
                {
                    while(await reader.ReadAsync())
                    {
                        ListOfLxc[reader.GetInt32(reader.GetOrdinal("lxcid"))] = true;
                    }
                }
            }
        }

        for(int i = 100; i < 256; i++)
        {
            if(ListOfLxc[i] == false)
            {
                await _proxmoxService.DeleteLXCAsync(i);
            }
        }

        _logger.LogInformation("Garbage collector completed...");
    }
}