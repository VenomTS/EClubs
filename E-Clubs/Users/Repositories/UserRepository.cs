using E_Clubs.Database;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.Users.Repositories;

public class UserRepository(AppDbContext dbContext)
{
    public async Task CreateUserAsync(User user)
    {
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task<User?> GetUserByMailAsync(string mail)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Mail == mail);
        return user;
    }

    public async Task<bool> UserExistsAsync(string mail) => 
        await dbContext.Users.AnyAsync(user => user.Mail == mail);
    
}