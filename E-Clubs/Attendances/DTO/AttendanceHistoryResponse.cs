using E_Clubs.Enums;

namespace E_Clubs.Attendances.DTO;

public class AttendanceHistoryResponse
{
    public DateOnly Date { get; set; }
    public AttendanceStatus Status { get; set; }
}