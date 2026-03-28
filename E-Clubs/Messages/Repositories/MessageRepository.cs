using E_Clubs.Database;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.Messages.Repositories;

public class MessageRepository(AppDbContext dbContext)
{
    public async Task<Message?> GetMessageByIdAsync(Guid id)
    {
        var message = await dbContext.Messages.Where(message => message.Id == id)
            .Include(message => message.Sender)
            .FirstOrDefaultAsync();

        return message;
    }
    
    public async Task<Message> CreateMessageAsync(Message message)
    {
        await dbContext.Messages.AddAsync(message);
        await dbContext.SaveChangesAsync();
        
        await dbContext.Entry(message).ReloadAsync();
        await dbContext.Entry(message).Reference(newMessage => newMessage.Sender).LoadAsync();
        return message;
    }

    public async Task<List<Message>> GetMessagesByClubIdAsync(Guid clubId)
    {
        var messages = await dbContext.Messages.Where(message => message.ClubId == clubId)
            .Include(message => message.Sender)
            .ToListAsync();

        return messages;
    }

    public async Task DeleteMessageAsync(Message message)
    {
        dbContext.Messages.Remove(message);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> UpdateMessageAsync(Guid messageId, Message newMessage)
    {
        var message = await dbContext.Messages
            .FirstOrDefaultAsync(message => message.Id == messageId);

        if (message == null)
            return false;
        
        message.Content = newMessage.Content;
        await dbContext.SaveChangesAsync();
        
        return true;
    }
}