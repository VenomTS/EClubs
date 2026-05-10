using E_Clubs.Enums;
using E_Clubs.Users.DTO;

namespace E_Clubs.Attendances.DTO;

public class GetUserAttendanceResponse
{
    public required GetUserResponse Student { get; set; }
    public DateOnly Date { get; set; }
    public AttendanceStatus Status { get; set; }
}