using E_Clubs.Messages;
using E_Clubs.Users;
using E_Clubs.WorkPlans;

namespace E_Clubs.Clubs;

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
    public required User Professor { get; set; }
    public List<WorkPlan> WorkPlans { get; set; } = [];
    public List<Message> Messages { get; set; } = [];
    public List<ClubStudent> ClubStudents { get; set; } = [];

}