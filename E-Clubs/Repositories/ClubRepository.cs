using E_Clubs.Data;
using E_Clubs.Models;
using E_Clubs.QueryObjects;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.Repositories;

public class ClubRepository(AppDbContext dbContext)
{

    public async Task<Club> CreateClubAsync(Club club)
    {
        await dbContext.Clubs.AddAsync(club);
        await dbContext.SaveChangesAsync();

        return club;
    }

    public async Task<IEnumerable<Club>> GetAllClubsAsync(GetAllClubsQueryObject queryObject)
    {
        IQueryable<Club> allClubs = dbContext.Clubs.AsNoTracking().AsQueryable().Include(club => club.Professor);
        
        // Trenutno radi samo za profesore
        // Potrebno dodati da radi za studente
        if (queryObject.UserId != null)
            allClubs = allClubs.Where(club => club.ProfessorId == queryObject.UserId);

        return await allClubs.ToListAsync();
    }

    public async Task<Club?> GetClubByIdAsync(Guid id) => await dbContext.Clubs
        .Include(club => club.Professor)
        .FirstOrDefaultAsync(club => club.Id == id);
}