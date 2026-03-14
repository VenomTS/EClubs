using E_Clubs.Data;
using E_Clubs.Models;

namespace E_Clubs.Repositories;

public class ClubRepository(AppDbContext dbContext)
{

    public async Task<Club> CreateClubAsync(Club club)
    {
        await dbContext.Clubs.AddAsync(club);
        await dbContext.SaveChangesAsync();

        return club;
    }

    public async Task<Club?> GetClubByIdAsync(Guid id) => await dbContext.Clubs.FindAsync(id);
}