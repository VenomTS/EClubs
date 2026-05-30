using E_Clubs.Reports.DTO;
using E_Clubs.Reports.QueryObject;
using E_Clubs.Reports.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Clubs.Reports;

[ApiController]
[Route("api/[controller]")]
public class ReportsController(ReportService reportService) : ControllerBase
{
    [HttpGet(Name = "GetReportsByClubId")]
    [ProducesResponseType<IEnumerable<GetReportsResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetByClubId([FromQuery] GetReportsQueryObject request)
    {
        var result = await reportService.GetReportsByClubIdAsync(request);

        return result.Match<ActionResult>(
            Ok,
            _ => NotFound(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"The club with ID {request.ClubId} was not found",
                Instance = HttpContext.Request.Path,
            }));
    }
}