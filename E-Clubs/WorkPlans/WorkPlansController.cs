using E_Clubs.WorkPlans.DTO;
using E_Clubs.WorkPlans.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Clubs.WorkPlans;

[ApiController]
[Route("api/clubs/{clubId:guid}/[controller]")]
public class WorkPlansController(WorkPlansService workPlansService) : ControllerBase
{

    [HttpGet(Name = "GetWorkPlansForClub")]
    public async Task<ActionResult<IEnumerable<GetAllWorkPlansByClubIdResponse>>> Get([FromRoute] Guid clubId)
    {
        var result = await workPlansService.GetAllWorkPlansByClubIdAsync(clubId);

        if (result.IsT0)
            return Ok(result.Value);
        return NotFound("Club not found");
    }

    [HttpPost(Name = "CreateWorkPlanForClub")]
    public async Task<ActionResult<CreateWorkPlanResponse>> Create([FromRoute] Guid clubId, [FromBody] CreateWorkPlanRequest request)
    {
        var result = await workPlansService.CreateWorkPlanAsync(clubId, request);

        if (result.IsT0)
            return Ok(result.Value);
        if (result.IsT1)
            return NotFound("Club not found");
        return Conflict("Same WorkPlan already exists for the given club");
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