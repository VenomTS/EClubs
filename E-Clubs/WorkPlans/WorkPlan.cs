using E_Clubs.Clubs;

namespace E_Clubs.WorkPlans;

public class WorkPlan
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }

    public string Domain { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string LearningOutcome { get; set; } = string.Empty;
    public string Indicator { get; set; } = string.Empty;
    
    public DateOnly? RealizationDate { get; set; }
    
    // Mapping Properties
    public required Club Club { get; set; }
}