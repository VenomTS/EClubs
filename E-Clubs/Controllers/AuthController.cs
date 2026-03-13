using E_Clubs.Data;
using E_Clubs.DTO.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.Controllers;

[ApiController]
[Route("/api/auth")]
public class AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : ControllerBase
{
    [HttpPost("/roles/{userId}")]
    public async Task<IActionResult> AssignRole(string userId, [FromBody] AssignRoleRequest roleRequest)
    {
        var role = await roleManager.Roles.FirstOrDefaultAsync(x => x.Name == roleRequest.Role);
        
        if(role == null)
            return BadRequest();
        
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();



        await userManager.AddToRoleAsync(user, role.Name!);
        return Ok();
    }
}