using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using TravelWithCode.Infrastructure;

namespace TravelWithCode.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]
public class GetGithubReposController : ControllerBase
{
    private readonly Postgresql _postgresql;
    private readonly Ciper _ciper;
    private readonly IHttpClientFactory _httpClientFactory;

    public GetGithubReposController(Postgresql postgresql, Ciper ciper, IHttpClientFactory httpClientFactory)
    {
        _postgresql = postgresql;
        _ciper = ciper;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("api/github/repos")]
    public async Task<IActionResult> GetGithubRepos()
    {
        var userID = HttpContext.Items["UserId"];
        var tokenVersion = HttpContext.Items["TokenVersion"];

        string encrypted = null;
        string iv = null;

        var listOfRepos = new List<string>();

        await using (var conn = await _postgresql.GetOpenConnectionAsync())
        {
            await using (var userData = new NpgsqlCommand("SELECT github_token, github_embedding FROM users WHERE id = @id AND token_version = @tokenVersion", conn))
            {
                userData.Parameters.AddWithValue("id", userID);
                userData.Parameters.AddWithValue("tokenVersion", tokenVersion);

                await using (var reader = await userData.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        encrypted = reader.GetString(reader.GetOrdinal("github_token"));
                        iv = reader.GetString(reader.GetOrdinal("github_embedding"));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }

        string token = _ciper.Decrypt(encrypted, iv);

        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized("A GitHub token hiányzik vagy nem dekódolható.");
        }

        try
        {
            var client = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/repos?type=owner");
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("travelwithcode", "1.0"));
            
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"GitHub API Hiba [{response.StatusCode}]: {errorBody}");
                return StatusCode((int)response.StatusCode, errorBody);
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            var jsonData = JsonNode.Parse(responseBody)?.AsArray();

            if (jsonData != null)
            {
                foreach (var repo in jsonData)
                {
                    listOfRepos.Add(repo["full_name"]?.ToString());
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            return StatusCode(500, "FATAL ERROR.");
        }

        return Ok(listOfRepos);
    }
}