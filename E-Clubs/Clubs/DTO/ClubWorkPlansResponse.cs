using E_Clubs.Enums;

namespace E_Clubs.Clubs.DTO;

public class ClubWorkPlansResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public WorkPlanStatus Status { get; set; }
    public DateOnly ScheduledDate { get; set; }
    public DateOnly? RealizationDate { get; set; }
}