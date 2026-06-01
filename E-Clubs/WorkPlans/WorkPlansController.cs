using E_Clubs.WorkPlans.DTO;
using E_Clubs.WorkPlans.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Clubs.WorkPlans;

[ApiController]
[Route("api/clubs/{clubId:guid}/[controller]")]
public class WorkPlansController(IWorkPlansService workPlansService) : ControllerBase
{

    [HttpGet(Name = "GetWorkPlansForClub")]
    [ProducesResponseType<IEnumerable<GetWorkPlanResponse>>(StatusCodes.Status200OK)]
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
    [ProducesResponseType<GetWorkPlanResponse>(StatusCodes.Status200OK)]
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

    [HttpPost("batch", Name = "BatchCreateWorkPlanForClub")]
    public async Task<ActionResult> BatchCreate([FromRoute] Guid clubId,
        [FromBody] IEnumerable<CreateWorkPlanRequest> request)
    {
        foreach(var requestItem in request)
            await Create(clubId, requestItem);
        
        return NoContent();
    }

    [HttpGet("current", Name = "GetCurrentWorkPlan")]
    [ProducesResponseType<GetWorkPlanResponse>(StatusCodes.Status200OK)]
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
    
    [HttpGet("domains", Name = "GetDomainsByClubId")]
    [ProducesResponseType<IEnumerable<GetDomainsResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetDomains([FromRoute] Guid clubId)
    {
        var result = await workPlansService.GetDomainsByClubIdAsync(clubId);

        return result.Match<ActionResult>(
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

    [HttpPost("upload", Name = "UploadWorkPlansForClub")]
    public async Task<ActionResult> Upload([FromRoute] Guid clubId, IFormFile file)
    {
        var result = await workPlansService.UploadWorkPlans(clubId, file);

        return result.Match<ActionResult>(
            _ => NoContent(),
            _ => NotFound(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"The club with ID {clubId} was not found",
                Instance = HttpContext.Request.Path,
            }),
            _ => BadRequest());
    }

    [HttpPost("conclude", Name = "ConcludeWorkPlanForClub")]
    public async Task<ActionResult> Conclude([FromRoute] Guid clubId, [FromBody] ConcludeWorkPlanRequest request)
    {
        var result = await workPlansService.ConcludeWorkPlan(clubId, request);

        return result.Match<ActionResult>(
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
                Detail = $"The Work Plan with ID {request.WorkPlanId} was not found",
                Instance = HttpContext.Request.Path,
            }));
    }
    
}