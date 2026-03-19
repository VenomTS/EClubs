using E_Clubs.WorkPlans.DTO;
using E_Clubs.WorkPlans.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Clubs.WorkPlans;

[ApiController]
[Route("api/clubs/{clubId:guid}/[controller]")]
public class WorkPlansController(WorkPlansService workPlansService) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetAllWorkPlansByClubIdResponse>>> Get([FromRoute] Guid clubId)
    {
        var result = await workPlansService.GetAllWorkPlansByClubIdAsync(clubId);

        if (result.IsT0)
            return Ok(result.Value);
        return NotFound("Club not found");
    }

    [HttpPost]
    public async Task<ActionResult<CreateWorkPlanResponse>> Create([FromRoute] Guid clubId, [FromBody] CreateWorkPlanRequest request)
    {
        var result = await workPlansService.CreateWorkPlanAsync(clubId, request);

        if (result.IsT0)
            return Ok(result.Value);
        if (result.IsT1)
            return NotFound("Club not found");
        return Conflict("Same WorkPlan already exists for the given club");
    }
}