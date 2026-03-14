using E_Clubs.Data;
using E_Clubs.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.Repositories;

public class UserRoleRepository(AppDbContext dbContext)
{
    public async Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId)
    {
        var roles = await dbContext.UserRoles
            .Where(userRole => userRole.UserId == userId)
            .Select(userRole => userRole.Role)
            .ToListAsync();

        return roles;
    }
    
}