using E_Clubs.Auth.DTO;
using E_Clubs.Auth.Services;
using E_Clubs.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Clubs.Auth;

[ApiController]
[Route("/api/[controller]")]
public class AuthController(IUserService userService) : ControllerBase
{
    private const string CookieName = "TOKEN";

    [HttpGet("me", Name = "GetMe")]
    [ProducesResponseType<GetMeResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<ActionResult> GetMe()
    {
        var token = Request.Cookies[CookieName];
        if(token == null)
            return Unauthorized();
        
        var result = await userService.GetMeAsync(token);
        
        return result.Match<ActionResult>(
            Ok, 
            _ => NotFound(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = "There is no user related to the token provided",
                Instance = HttpContext.Request.Path,
            })
        );
    }
    
    [HttpPost("register", Name = "RegisterUser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Register([FromBody] RegisterUserRequest registerUserRequest)
    {
        var result = await userService.RegisterUserAsync(registerUserRequest);

        if (result.IsT1)
            return Conflict(new ProblemDetails
            {
                Type = "User-Already-Exists",
                Title = "Conflict",
                Status = StatusCodes.Status409Conflict,
                Detail = "A user with this email already exists",
                Instance = HttpContext.Request.Path
            });
        
        Response.Cookies.Append(CookieName, result.AsT0, GetCookieOptions());
        return NoContent();
    }

    [HttpPost("login", Name = "LoginUser")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest loginUserRequest)
    {
        var result = await userService.LoginAsync(loginUserRequest);

        if (!result.IsT0) return Unauthorized();
        
        Response.Cookies.Append(CookieName, result.AsT0, GetCookieOptions());
        return NoContent();
    }

    private static CookieOptions GetCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = JWTService.GetExpirationDate(),
        };
    }
}