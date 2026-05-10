using E_Clubs.Users.DTO;

namespace E_Clubs.Clubs.DTO;

public class GetClubResponse
{
    public Guid Id { get; set; }
    public GetUserResponse Professor { get; set; }
    public string Name { get; set; } = string.Empty;
}