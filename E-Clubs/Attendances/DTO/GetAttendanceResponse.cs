using E_Clubs.Users.DTO;

namespace E_Clubs.Attendances.DTO;

public class GetAttendanceResponse
{
    public required GetUserResponse Student { get; set; }
    public required List<AttendanceHistoryResponse> AttendanceHistory { get; set; }
}