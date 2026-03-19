namespace E_Clubs.WorkPlans.DTO;

public class CreateWorkPlanRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public DateOnly ScheduledDate { get; set; }
}