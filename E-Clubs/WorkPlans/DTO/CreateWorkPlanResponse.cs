using E_Clubs.Clubs;
using E_Clubs.Enums;

namespace E_Clubs.WorkPlans.DTO;

public class CreateWorkPlanResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public WorkPlanStatus Status { get; set; } = WorkPlanStatus.Scheduled;
    public DateOnly ScheduledDate { get; set; }
}