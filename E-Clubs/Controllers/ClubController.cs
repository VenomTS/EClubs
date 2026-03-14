using E_Clubs.Data;
using E_Clubs.DTO.ClubDTO;
using E_Clubs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Clubs.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ClubController(ClubService clubService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = AppRoles.Professor)]
    public async Task<ActionResult<CreateClubResponse>> Create([FromBody] CreateClubRequest request)
    {
        var createdClub = await clubService.CreateClubAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = createdClub.Id }, createdClub);
    }

    // public async Task GetAll()
    // {
    //     
    // }
    //
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetClubByIdResponse>> GetById(Guid id)
    {
        var result = await clubService.GetClubByIdAsync(id);

        return result.Match<ActionResult<GetClubByIdResponse>>(club => Ok(club), _ => NotFound("Club not found"));
    }
    //
    // public async Task Update()
    // {
    //     
    // }
    //
    // public async Task Delete(int id)
    // {
    //     
    // }
}