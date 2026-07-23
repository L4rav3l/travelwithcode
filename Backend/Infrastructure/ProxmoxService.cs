using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace TravelWithCode.Infrastructure;

public class ProxmoxService
{
    private readonly HttpClient _client;
    private readonly string _url;
    private readonly string _token;
    private readonly string _datacenter;

    public ProxmoxService(HttpClient client)
    {
        _client = client;
        _url = Environment.GetEnvironmentVariable("PROXMOX_URL");
        _token = Environment.GetEnvironmentVariable("PROXMOX_TOKEN");
        _datacenter = Environment.GetEnvironmentVariable("PROXMOX_DATACENTER");
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string path, HttpContent content = null)
    {
        var request = new HttpRequestMessage(method, $"{_url}{path}");
        request.Headers.TryAddWithoutValidation("Authorization", _token);

        if(content != null)
        {
            request.Content = content;
        }

        return request;
    }

    public async Task<int> NextIdAsync()
    {
        var request = CreateRequest(HttpMethod.Get, "/api2/json/cluster/nextid");
        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonNode.Parse(body);

        return Convert.ToInt32(json["data"]?.ToString());
    }

    public async Task CreateLXCAsync(int lxcId)
    {
        string jsonBody = $"{{\"vmid\": {lxcId}, \"ostemplate\" : \"local:vztmpl/travelwithcode.tar.zst\", \"rootfs\": \"local-lvm:25\", \"cores\": 1, \"memory\": 2048, \"swap\": 2048, \"password\": \"03Q@bD96GzWv\", \"net0\": \"name=eth0,bridge=vmbr0,firewall=1,ip=192.168.122.{lxcId}/24,gw=192.168.122.1\"}}";
        var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");

        var request = CreateRequest(HttpMethod.Post, $"/api2/json/nodes/{_datacenter}/lxc", content);
        
        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task StartLXCAsync(int lxcId)
    {
        var request = CreateRequest(HttpMethod.Post, $"/api2/json/nodes/{_datacenter}/lxc/{lxcId}/status/start");
        
        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteLXCAsync(int lxcId)
    {
        try
        {
            var request = CreateRequest(HttpMethod.Delete, $"/api2/json/nodes/{_datacenter}/lxc/{lxcId}?purge=0&destroy-unreferenced-disks=0");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        catch(Exception ex)
        {
            
        }
    }
}