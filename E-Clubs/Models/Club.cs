namespace E_Clubs.Models;

public class Club
{
    
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid ProfessorId { get; set; }
    public DayOfWeek Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsActive { get; set; }
    public DateOnly CreatedAt { get; set; }
    
    // Mapping Properties
    public User Professor { get; set; }

}