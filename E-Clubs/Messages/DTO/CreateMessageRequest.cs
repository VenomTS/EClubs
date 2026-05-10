namespace E_Clubs.Messages.DTO;

public class CreateMessageRequest
{
    public Guid SenderId { get; set; }
    
    public string Content { get; set; } = string.Empty;
}