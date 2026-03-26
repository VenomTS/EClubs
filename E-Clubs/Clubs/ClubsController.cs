using E_Clubs.Clubs.DTO;
using E_Clubs.Clubs.QueryObjects;
using E_Clubs.Clubs.Services;
using E_Clubs.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Clubs.Clubs;

[ApiController]
[Route("/api/[controller]")]
public class ClubsController(ClubService clubService) : ControllerBase
{
    [HttpPost(Name = "CreateClub")]
    public async Task<ActionResult<CreateClubResponse>> Create([FromBody] CreateClubRequest request)
    {
        var createdClub = await clubService.CreateClubAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = createdClub.Id }, createdClub);
    }

    [HttpGet(Name = "GetClubsForUser")]
    public async Task<ActionResult<GetAllClubsResponse>> GetAll([FromQuery] GetAllClubsQueryObject queryObject)
    {
        var clubs = await clubService.GetAllClubsAsync(queryObject);

        return Ok(clubs);
    }
    
    [HttpGet("{id:guid}", Name = "GetClubById")]
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