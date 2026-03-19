using E_Clubs.Clubs;
using E_Clubs.Enums;

namespace E_Clubs.WorkPlans;

public class WorkPlan
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public WorkPlanStatus Status { get; set; }
    public DateOnly ScheduledDate { get; set; }
    public DateOnly? RealizationDate { get; set; }
    
    // Mapping Properties
    public required Club Club { get; set; }
}