using E_Clubs.Users.DTO;
using E_Clubs.WorkPlans.DTO;

namespace E_Clubs.Reports.DTO;

public class GetReportsResponse
{
    public Guid Id { get; set; }

    public GetWorkPlanResponse WorkPlan { get; set; }
    public GetUserResponse Professor { get; set; }
    
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public string Present { get; set; } = string.Empty;
    public string Absent { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
}