using Renci.SshNet;
using System;

namespace TravelWithCode.Infrastructure;

public class SSHService
    {
    public async Task<SshClient> CreateSSHConnection(string host, string username, string password)
    {
        var client = new SshClient(host, username, password);

        try
        {
            client.Connect();
            return client;
        }
        catch (Exception ex)
        {
            client.Dispose(); 
            return null;
        }
    }
}