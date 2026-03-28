namespace E_Clubs.Messages.DTO;

public class GetMessageByIdResponse
{
    public Guid Id { get; set; }

    public required MessageSenderResponse Sender { get; set; }
    
    public string Content { get; set; } = string.Empty;
    // public List<Attachment> Attachments { get; set; }

    public DateTime SentAt { get; set; }
}