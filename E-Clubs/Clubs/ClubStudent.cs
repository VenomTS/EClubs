using E_Clubs.Users;

namespace E_Clubs.Clubs;

public class ClubStudent
{
    public Guid ClubId { get; set; }
    public Guid StudentId { get; set; }

    public required Club Club { get; set; }
    public required User Student { get; set; }
}