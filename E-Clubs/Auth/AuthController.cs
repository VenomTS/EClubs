using E_Clubs.Users.DTO;
using E_Clubs.Users.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Clubs.Auth;

[ApiController]
[Route("/api/[controller]")]
public class AuthController(UserService userService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Register([FromBody] RegisterUserRequest registerUserRequest)
    {
        var result = await userService.RegisterUserAsync(registerUserRequest);

        return result.Match<ActionResult>(_ => Created(), _ => Conflict(new ProblemDetails
        {
            Type = "User-Already-Exists",
            Title = "Conflict",
            Status = StatusCodes.Status409Conflict,
            Detail = "A user with this email already exists",
            Instance = HttpContext.Request.Path
        }));
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<LoginUserResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<LoginUserResponse>> Login([FromBody] LoginUserRequest loginUserRequest)
    {
        var result = await userService.LoginAsync(loginUserRequest);
        
        return result.Match<ActionResult>(Ok, _ => Unauthorized("Invalid mail or password"));
    }
}