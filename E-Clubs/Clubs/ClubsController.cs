using E_Clubs.Clubs.DTO;
using E_Clubs.Clubs.QueryObjects;
using E_Clubs.Clubs.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Clubs.Clubs;

[ApiController]
[Route("/api/[controller]")]
public class ClubsController(ClubService clubService) : ControllerBase
{
    [HttpGet("{id:guid}", Name = "GetClubById")]
    [ProducesResponseType<GetClubByIdResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await clubService.GetClubByIdAsync(id);

        return result.Match<IActionResult>(
            Ok,
            _ => NotFound(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"The club with ID {id} was not found",
                Instance = HttpContext.Request.Path,
            })
        );
    }
    
    [HttpPost(Name = "CreateClub")]
    [ProducesResponseType<CreateClubResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateClubRequest request)
    {
        var createdClub = await clubService.CreateClubAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = createdClub.Id }, createdClub);
    }

    [HttpGet(Name = "GetClubsForUser")]
    [ProducesResponseType<IEnumerable<GetAllClubsResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllClubsQueryObject queryObject)
    {
        var clubs = await clubService.GetAllClubsAsync(queryObject);

        return Ok(clubs);
    }

    [HttpPost("{clubId:guid}/students/{studentId:guid}", Name = "AddStudentToClub")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddStudent([FromRoute] Guid clubId, [FromRoute] Guid studentId)
    {
        var result = await clubService.AddStudentToClubAsync(clubId, studentId);

        return result.Match<IActionResult>(
            _ => NoContent(),
            _ => NotFound(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"The club with ID {clubId} was not found",
                Instance = HttpContext.Request.Path,
            }),
            _ => NotFound(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"The student with ID {studentId} was not found",
                Instance = HttpContext.Request.Path,
            }),
            _ => Conflict(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
                Title = "Conflict",
                Status = StatusCodes.Status409Conflict,
                Detail = $"The student with ID {studentId} is already in the club with ID {clubId}",
                Instance = HttpContext.Request.Path,
            })
        );
    }

    [HttpDelete("{clubId:guid}/students/{studentId:guid}", Name = "DeleteStudentFromClub")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudent([FromRoute] Guid clubId, [FromRoute] Guid studentId)
    {
        var result = await clubService.DeleteStudentFromClub(clubId, studentId);
        
        return result.Match<IActionResult>(
            _ => NoContent(),
            _ => NotFound(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"The club with ID {clubId} was not found",
                Instance = HttpContext.Request.Path,
            }),
            _ => NotFound(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"The student with ID {studentId} was not found",
                Instance = HttpContext.Request.Path,
            }),
            _ => NoContent()
        );
    }
    
    [HttpPost("import", Name = "ImportClubs")]
    public async Task<IActionResult> Import(IFormFile file)
    {
        return BadRequest("Feature not implemented");
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