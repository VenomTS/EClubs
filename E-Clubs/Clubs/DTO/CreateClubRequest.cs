namespace E_Clubs.Clubs.DTO;

public class CreateClubRequest
{
    public string Name { get; set; } = string.Empty;
    public Guid ProfessorId { get; set; }
    public DayOfWeek Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}