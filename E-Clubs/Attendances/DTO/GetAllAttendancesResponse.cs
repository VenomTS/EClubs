using E_Clubs.Enums;

namespace E_Clubs.Attendances.DTO;

public class GetAllAttendancesResponse
{
    public Guid UserId { get; set; }
    public DateOnly Date { get; set; }
    public AttendanceStatus Status { get; set; }
}