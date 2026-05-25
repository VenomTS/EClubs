namespace E_Clubs.Clubs.DTO;

public class JoinClubRequest
{
    public Guid StudentId { get; set; }
    public string Code { get; set; } = string.Empty;
}