using E_Clubs.Attendances.DTO;
using E_Clubs.Attendances.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Clubs.Attendances;

[ApiController]
[Route("api/clubs/{clubId:guid}/[controller]")]
public class AttendancesController(AttendanceService attendanceService) : ControllerBase
{

    // This one should only be accessible by the professor
    [HttpGet(Name = "GetAttendancesForClub")]
    public async Task<ActionResult<List<GetAllAttendancesResponse>>> GetAll([FromRoute] Guid clubId)
    {
        var result = await attendanceService.GetAllAttendancesByClubIdAsync(clubId);

        if (result.IsT0)
            return Ok(result.Value);
        return NotFound("Club not found");
    }

    // This one should only be accessible by the student
    [HttpGet("{userId:guid}", Name = "GetUserAttendancesForClub")]
    public async Task<ActionResult<List<GetUserAttendanceResponse>>> Get([FromRoute] Guid clubId, [FromRoute] Guid userId)
    {
        var result = await attendanceService.GetUserAttendanceByClubIdAsync(clubId, userId);

        if (result.IsT0)
            return Ok(result.Value);
        if (result.IsT1)
            return NotFound("Club not found");
        return NotFound("User not found");
    }
    
    [HttpPost(Name = "MarkStudentPresent")]
    public async Task<IActionResult> Register([FromRoute] Guid clubId, [FromBody] RegisterAttendanceRequest request)
    {
        var result = await attendanceService.RegisterAttendanceAsync(clubId, request);

        if (result.IsT0)
            return Ok();
        if (result.IsT1)
            return NotFound("Club not found");
        return NotFound("User not found");
    }

    // Call this when finished taking attendance
    [HttpPost("conclude", Name = "ConcludeAttendanceTaking")]
    public async Task<IActionResult> ConcludeRegistration([FromRoute] Guid clubId)
    {
        var result = await attendanceService.MarkAbsentStudentsAsync(clubId);

        if (result.IsT0)
            return Ok();
        return NotFound("Club not found");
    }
}