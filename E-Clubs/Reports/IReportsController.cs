using E_Clubs.Reports.DTO;
using E_Clubs.Reports.QueryObject;
using E_Clubs.Reports.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Clubs.Reports;

public interface IReportsController
{
    [HttpGet(Name = "GetReportsByClubId")]
    [ProducesResponseType<IEnumerable<GetReportsResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    Task<ActionResult> GetByClubId([FromQuery] GetReportsQueryObject request);
}