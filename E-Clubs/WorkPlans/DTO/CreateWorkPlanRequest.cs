namespace E_Clubs.WorkPlans.DTO;

public class CreateWorkPlanRequest
{
    public int DomainNumber { get; set; }
    public string Domain { get; set; } = string.Empty;
    public string LessonUnit { get; set; } = string.Empty;
    public string LearningOutcome { get; set; } = string.Empty;
    public string Indicator { get; set; } = string.Empty;
    public DateOnly ScheduledDate { get; set; }
}