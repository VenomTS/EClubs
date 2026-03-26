namespace E_Clubs.Clubs.DTO;

public class CreateClubResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public required ClubProfessorResponse Professor { get; set; }
}