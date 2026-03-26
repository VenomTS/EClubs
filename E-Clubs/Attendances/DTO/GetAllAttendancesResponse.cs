namespace E_Clubs.Attendances.DTO;

public class GetAllAttendancesResponse
{
    public required AttendanceUserResponse Student { get; set; }
    public required List<AttendanceHistoryResponse> AttendanceHistory { get; set; }
}