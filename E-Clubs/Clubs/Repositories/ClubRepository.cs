using E_Clubs.Clubs.QueryObjects;
using E_Clubs.Database;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.Clubs.Repositories;

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
        .Include(club => club.WorkPlans)
        .FirstOrDefaultAsync(club => club.Id == id);
}