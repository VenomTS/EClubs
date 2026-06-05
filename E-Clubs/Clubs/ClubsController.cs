using E_Clubs.Clubs.DTO;
using E_Clubs.Clubs.QueryObjects;
using E_Clubs.Clubs.Services;
using E_Clubs.Enums;
using E_Clubs.Users.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Clubs.Clubs;

[ApiController]
[Route("/api/[controller]")]
public class ClubsController(IClubService clubService) : ControllerBase
{
    [HttpGet("{id:guid}", Name = "GetClubById")]
    [ProducesResponseType<GetClubResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [Authorize(Roles = $"{nameof(Roles.Student)}, {nameof(Roles.Professor)}, {nameof(Roles.Director)}")]
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
    [ProducesResponseType<GetClubResponse>(StatusCodes.Status201Created)]
    [Authorize(Roles = $"{nameof(Roles.Professor)}")]
    public async Task<IActionResult> Create([FromBody] CreateClubRequest request)
    {
        var createdClub = await clubService.CreateClubAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = createdClub.Id }, createdClub);
    }

    [HttpGet(Name = "GetClubsForUser")]
    [Authorize(Roles = $"{nameof(Roles.Student)}, {nameof(Roles.Professor)}")]
    [ProducesResponseType<IEnumerable<GetClubResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetClubsQueryObject queryObject)
    {
        var clubs = await clubService.GetClubsByUserIdAsync(queryObject);

        return Ok(clubs);
    }

    [HttpGet("{clubId:guid}/students", Name = "GetStudentsInClub")]
    [ProducesResponseType<IEnumerable<GetUserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [Authorize(Roles = $"{nameof(Roles.Professor)}, {nameof(Roles.Director)}")]
    public async Task<IActionResult> GetStudents([FromRoute] Guid clubId)
    {
        var result = await clubService.GetStudentsByClubIdAsync(clubId);

        return result.Match<IActionResult>(
            Ok,
            _ => NotFound(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"The club with ID {clubId} was not found",
                Instance = HttpContext.Request.Path,
            })
        );
    }

    [HttpPost("join", Name = "AddStudentToClub")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [Authorize(Roles = $"{nameof(Roles.Student)}")]
    public async Task<IActionResult> AddStudent([FromBody] JoinClubRequest request)
    {
        var result = await clubService.AddStudentToClubAsync(request);

        return result.Match<IActionResult>(
            _ => NoContent(),
            _ => NotFound(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"The club with Code {request.Code} was not found",
                Instance = HttpContext.Request.Path,
            }),
            _ => NotFound(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"The student with ID {request.StudentId} was not found",
                Instance = HttpContext.Request.Path,
            }),
            _ => Conflict(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
                Title = "Conflict",
                Status = StatusCodes.Status409Conflict,
                Detail = $"The student with ID {request.StudentId} is already in the club",
                Instance = HttpContext.Request.Path,
            })
        );
    }

    [HttpDelete("{clubId:guid}/students", Name = "DeleteStudentFromClub")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [Authorize(Roles = $"{nameof(Roles.Student)}, {nameof(Roles.Professor)}")]
    public async Task<IActionResult> DeleteStudent([FromRoute] Guid clubId, [FromBody] KickStudentRequest request)
    {
        var result = await clubService.DeleteStudentFromClub(clubId, request);
        
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
                Detail = $"The student with ID {request.StudentId} was not found",
                Instance = HttpContext.Request.Path,
            }),
            _ => NoContent()
        );
    }
}