namespace E_Clubs.DTO.ClubDTO;

public class GetAllClubsResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public ClubProfessorResponse Professor { get; set; }
}