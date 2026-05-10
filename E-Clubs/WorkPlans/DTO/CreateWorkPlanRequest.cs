namespace E_Clubs.WorkPlans.DTO;

public class CreateWorkPlanRequest
{
    public string Domain { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string LearningOutcome { get; set; } = string.Empty;
    public string Indicator { get; set; } = string.Empty;
    public DateOnly ScheduledDate { get; set; }
}