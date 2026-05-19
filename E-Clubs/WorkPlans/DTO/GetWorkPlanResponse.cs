namespace E_Clubs.WorkPlans.DTO;

public class GetWorkPlanResponse
{
    public Guid Id { get; set; }

    public string Domain { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string LearningOutcome { get; set; } = string.Empty;
    public string Indicator { get; set; } = string.Empty;
    
    public DateOnly? RealizationDate { get; set; }
}