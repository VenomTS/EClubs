namespace E_Clubs.WorkPlans.DTO;

public class GetCurrentWorkPlanResponse
{
    public Guid Id { get; set; }
    public int DomainNumber { get; set; }
    public string Domain { get; set; } = string.Empty;
    public int LessonNumber { get; set; }
    public string LessonUnit { get; set; } = string.Empty;
    public string LearningOutcome { get; set; } = string.Empty;
    public string Indicator { get; set; } = string.Empty;
}