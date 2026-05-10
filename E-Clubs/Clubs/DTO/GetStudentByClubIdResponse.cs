namespace E_Clubs.Clubs.DTO;

public class GetStudentByClubIdResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}