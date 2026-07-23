using Microsoft.AspNetCore.Mvc;
using TravelWithCode.Infrastructure;

namespace TravelWithCode.Controllers;

[ServiceFilter(typeof(AuthorizationFilter))]
[ApiController]
public class TokenVerifyController : ControllerBase
{   
    [HttpGet("api/auth/verify")]
    public IActionResult Verify()
    {
        var userID = HttpContext.Items["UserId"];

        if(userID != null)
        {
            return Ok();
        } else {
            return Unauthorized();
        }
    }
}