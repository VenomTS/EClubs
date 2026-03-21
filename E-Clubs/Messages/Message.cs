using E_Clubs.Clubs;
using E_Clubs.Users;

namespace E_Clubs.Messages;

public class Message
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid ClubId { get; set; }
    
    public string Content { get; set; } = string.Empty;
    // public List<Attachment> Attachments { get; set; }
    public DateTime SentAt { get; set; }
    
    // Mappings
    public required User Sender { get; set; }
    public required Club Club { get; set; }
}