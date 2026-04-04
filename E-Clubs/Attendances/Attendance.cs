using E_Clubs.Clubs;
using E_Clubs.Enums;
using E_Clubs.Users;
using E_Clubs.WorkPlans;

namespace E_Clubs.Attendances;

public class Attendance
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public Guid WorkPlanId { get; set; }
    public Guid StudentId { get; set; }
    public DateOnly Date { get; set; }
    public AttendanceStatus Status { get; set; }

    public required Club Club { get; set; }
    public required WorkPlan WorkPlan { get; set; }
    public required User Student { get; set; }
}