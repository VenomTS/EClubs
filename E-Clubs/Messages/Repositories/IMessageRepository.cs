using E_Clubs.Database;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.Messages.Repositories;

public interface IMessageRepository
{
    Task<Message?> GetMessageByIdAsync(Guid id);
    Task<Message> CreateMessageAsync(Message message);
    Task<List<Message>> GetMessagesByClubIdAsync(Guid clubId);
    Task DeleteMessageAsync(Message message);
    Task<bool> UpdateMessageAsync(Guid messageId, Message newMessage);
}