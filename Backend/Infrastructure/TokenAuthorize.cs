using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TravelWithCode.Infrastructure;
using Npgsql;

namespace TravelWithCode.Infrastructure;

public class AuthorizationFilter : IAsyncAuthorizationFilter
{
    private readonly Postgresql _postgresql;
    private readonly JsonWebToken _jsonwebtoken;

    public AuthorizationFilter(Postgresql postgresql, JsonWebToken jsonwebtoken)
    {
        _postgresql = postgresql;
        _jsonwebtoken = jsonwebtoken;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var request = context.HttpContext.Request;

        if(!request.Headers.ContainsKey("Authorization"))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var token = request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        var informationOfToken = _jsonwebtoken.VerifyToken(token);

        if(!informationOfToken.HasValue)
        {
            context.Result = new UnauthorizedResult();
            return;
        } else {
            int? id = informationOfToken.Value.id;
            int? tokenVersion = informationOfToken.Value.tokenVersion;

            await using(var conn = await _postgresql.GetOpenConnectionAsync())
            {
                await using(var checkUser = new NpgsqlCommand("SELECT * FROM users WHERE id = @id AND token_version = @tokenVersion", conn))
                {
                    checkUser.Parameters.AddWithValue("id", id);
                    checkUser.Parameters.AddWithValue("tokenVersion", tokenVersion);

                    await using(var reader = await checkUser.ExecuteReaderAsync())
                    {
                        if(await reader.ReadAsync())
                        {
                            context.HttpContext.Items["UserId"] = id;
                            context.HttpContext.Items["TokenVersion"] = tokenVersion;
                        } else {
                            context.Result = new UnauthorizedResult();
                            return;
                        }
                    }
                }
            }
        }
    }
}