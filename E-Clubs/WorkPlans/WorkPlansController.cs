using E_Clubs.WorkPlans.DTO;
using E_Clubs.WorkPlans.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Clubs.WorkPlans;

[ApiController]
[Route("api/clubs/{clubId:guid}/[controller]")]
public class WorkPlansController(WorkPlansService workPlansService) : ControllerBase
{

    [HttpGet(Name = "GetWorkPlansForClub")]
    [ProducesResponseType<IEnumerable<GetAllWorkPlansByClubIdResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid clubId)
    {
        var result = await workPlansService.GetAllWorkPlansByClubIdAsync(clubId);

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

    [HttpPost(Name = "CreateWorkPlanForClub")]
    [ProducesResponseType<IEnumerable<CreateWorkPlanResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromRoute] Guid clubId, [FromBody] CreateWorkPlanRequest request)
    {
        var result = await workPlansService.CreateWorkPlanAsync(clubId, request);

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

    [HttpGet("/current", Name = "GetCurrentWorkPlan")]
    [ProducesResponseType<GetCurrentWorkPlanResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetCurrentWorkPlan([FromRoute] Guid clubId)
    {
        var result = await workPlansService.GetCurrentWorkPlanByClubIdAsync(clubId);
        
        return result.Match<IActionResult>(
            Ok,
            _ => NotFound(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"The club with ID {clubId} was not found",
                Instance = HttpContext.Request.Path,
            }),
            _ => NoContent()
        );
    }
    
}