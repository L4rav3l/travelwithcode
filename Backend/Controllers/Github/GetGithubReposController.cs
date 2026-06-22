using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;
using Npgsql;


namespace TravelWithCode.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]
public class GetGithubReposController : ControllerBase
{
    private readonly Postgresql _postgresql;
    private readonly Ciper _ciper;
    private readonly HttpClient _client;

    public GetGithubReposController(Postgresql postgresql, Ciper ciper)
    {
        _postgresql = postgresql;
        _ciper = ciper;
        _client = new HttpClient();
    }

    [HttpGet("api/github/repos")]
    public async Task<IActionResult> GetGithubRepos()
    {
        var userID = HttpContext.Items["UserId"];
        var tokenVersion = HttpContext.Items["TokenVersion"];

        string encrypted = null;
        string iv = null;

        List<string> listOfRepos = new List<string>();

        await using(var conn = await _postgresql.GetOpenConnectionAsync())
        {
            await using(var userData = new NpgsqlCommand("SELECT * FROM users WHERE id = @id AND token_version = @tokenVersion", conn))
            {
                userData.Parameters.AddWithValue("id", userID);
                userData.Parameters.AddWithValue("tokenVersion", tokenVersion);

                await using(var reader = await userData.ExecuteReaderAsync())
                {
                    if(await reader.ReadAsync())
                    {
                        encrypted = reader.GetString(reader.GetOrdinal("github_token"));
                        iv = reader.GetString(reader.GetOrdinal("github_embedding"));
                    } else {
                        return NotFound();
                    }
                }
            }
        }

        string token = _ciper.Decrypt(encrypted, iv);

        try
        {
            _client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("travelwithcode", "1.0"));
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _client.GetAsync("https://api.github.com/user/repos?type=owner");
            
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            var jsonData = System.Text.Json.Nodes.JsonNode.Parse(responseBody).AsArray();

            foreach(var repo in jsonData)
            {
                listOfRepos.Add(repo["full_name"].ToString());
            }
        }

        catch(Exception ex)
        {
            Console.WriteLine(ex);
            return Unauthorized();
        }

        return Ok(listOfRepos);
    }
}