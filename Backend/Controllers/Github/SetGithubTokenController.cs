using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;
using TravelWithCode.Request;
using Npgsql;

namespace TravelWithCode.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]
public class SetGithubTokenController : ControllerBase
{   
    private readonly Ciper _ciper;
    private readonly Postgresql _postgresql;

    public SetGithubTokenController(Ciper ciper, Postgresql postgresql)
    {
        _ciper = ciper;
        _postgresql = postgresql;
    }

    [HttpPost("api/github/set_token")]
    public async Task<IActionResult> SetGithubToken([FromBody] SetGithubTokenRequest request)
    {
        var userID = (int)HttpContext.Items["UserId"];
        var tokenVersion = (int)HttpContext.Items["TokenVersion"];

        var encrypt = _ciper.Encrypt(request.githubToken);
        
        string encryptedText = encrypt.encrypted;
        string ivText = encrypt.IV;

        await using(var conn = await _postgresql.GetOpenConnectionAsync())
        {
            await using(var updateUsers = new NpgsqlCommand("UPDATE users SET github_token = @githubToken, github_embedding = @githubIv WHERE id = @id AND token_version = @tokenVersion", conn))
            {
                updateUsers.Parameters.AddWithValue("githubToken", encryptedText);
                updateUsers.Parameters.AddWithValue("githubIv", ivText);
                updateUsers.Parameters.AddWithValue("id", userID);
                updateUsers.Parameters.AddWithValue("tokenVersion", tokenVersion);

                await updateUsers.ExecuteNonQueryAsync();
            }
        }

        return Ok(new {status = 1});
    }
    
}