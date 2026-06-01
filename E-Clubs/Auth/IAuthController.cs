using E_Clubs.Auth.DTO;
using E_Clubs.Auth.Services;
using E_Clubs.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Clubs.Auth;

public interface IAuthController
{
    Task<ActionResult> GetMe();

    Task<ActionResult> Register([FromBody] RegisterUserRequest registerUserRequest);
    Task<IActionResult> Login([FromBody] LoginUserRequest loginUserRequest);

    CookieOptions GetCookieOptions();
}