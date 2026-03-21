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

        return message;
    }

    public async Task<List<Message>> GetMessagesByClubIdAsync(Guid clubId)
    {
        var messages = await dbContext.Messages.Where(message => message.ClubId == clubId).ToListAsync();

        return messages;
    }
}