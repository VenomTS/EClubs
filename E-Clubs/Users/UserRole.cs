using E_Clubs.Auth;

namespace E_Clubs.Users;

public class UserRole
{
    public Guid UserId { get; set; }
    public required User User { get; set; }
    
    public Guid RoleId { get; set; }
    public required Role Role { get; set; }
}