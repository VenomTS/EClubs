namespace E_Clubs.WorkPlans.DTO;

public class ConcludeWorkPlanRequest
{
    public Guid WorkPlanId { get; set; }
    public DateOnly Date { get; set; }
}