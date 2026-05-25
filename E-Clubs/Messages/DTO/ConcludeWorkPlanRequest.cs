namespace E_Clubs.Messages.DTO;

public class ConcludeWorkPlanRequest
{
    public Guid WorkPlanId { get; set; }
    public DateOnly Date { get; set; }
}