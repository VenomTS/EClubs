namespace E_Clubs.Clubs.DTO;

public class GetClubByIdResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DayOfWeek Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsActive { get; set; }
    public DateOnly CreatedAt { get; set; }

    public required ClubProfessorResponse Professor { get; set; }
    public required List<ClubWorkPlansResponse> WorkPlans { get; set; }
}