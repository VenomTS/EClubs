using E_Clubs.DTO.UserDTO;
using E_Clubs.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Clubs.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthController(UserService userService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterUserRequest registerUserRequest)
    {
        var result = await userService.RegisterUserAsync(registerUserRequest);

        return result.Match<ActionResult>(_ => Created(), _ => Conflict("User already exists"));
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginUserRequest loginUserRequest)
    {
        var result = await userService.LoginAsync(loginUserRequest);
        
        return result.Match<ActionResult>(Ok, _ => NotFound("Invalid mail or password"));
    }
}