namespace E_Clubs.Attendances.DTO;

public class RegisterAttendanceRequest
{
    public Guid WorkPlanId { get; set; }
    public Guid StudentId { get; set; }
}