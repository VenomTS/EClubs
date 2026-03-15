namespace E_Clubs.Users.DTO;

public class RegisterUserRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}