using E_Clubs.Users.DTO;

namespace E_Clubs.Messages.DTO;

public class GetMessageResponse
{
    public Guid Id { get; set; }
    public required GetUserResponse Sender { get; set; }
    
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}