using E_Clubs.Database;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.Users.Repositories;

public interface IUserRepository
{
    Task CreateUserAsync(User user);

    Task<User?> GetUserByMailAsync(string mail);
    
    Task<User?> GetUserByIdAsync(Guid userId);

    Task<bool> UserExistsAsync(string mail);
    
    Task<bool> UserExistsAsync(Guid userId);
}