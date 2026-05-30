using E_Clubs.Clubs;
using E_Clubs.Users;
using E_Clubs.WorkPlans;

namespace E_Clubs.Reports;

public class Report
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public Guid WorkPlanId { get; set; }
    public Guid ProfessorId { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public string Present { get; set; } = string.Empty;
    public string Absent { get; set; } = string.Empty;
    public DateOnly Date { get; set; }

    public Club Club { get; set; }
    public WorkPlan WorkPlan { get; set; }
    public User Professor { get; set; }
}