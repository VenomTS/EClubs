using E_Clubs.Enums;

namespace E_Clubs.Clubs.DTO;

public class ClubWorkPlansResponse
{
    public Guid Id { get; set; }
    
    public int DomainNumber { get; set; }
    public string Domain { get; set; } = string.Empty;
    public int LessonNumber { get; set; }
    public string LessonUnit { get; set; } = string.Empty;
    public string LearningOutcome { get; set; } = string.Empty;
    public string Indicator { get; set; } = string.Empty;
    
    public WorkPlanStatus Status { get; set; }
    public DateOnly ScheduledDate { get; set; }
    public DateOnly? RealizationDate { get; set; }
}