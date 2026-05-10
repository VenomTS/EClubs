using E_Clubs.Database;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.Clubs.Repositories;

public class ClubRepository(AppDbContext dbContext)
{

    public async Task<Club> CreateClubAsync(Club club)
    {
        await dbContext.Clubs.AddAsync(club);
        await dbContext.SaveChangesAsync();
        
        var createdClub = await GetClubByIdAsync(club.Id);
        return createdClub ?? throw new Exception("Club was not created");
    }

    public async Task<IEnumerable<Club>> GetClubsByUserIdAsync(Guid? userId)
    {
        var clubs = dbContext.Clubs.AsNoTracking()
            .Include(club => club.Professor)
            .AsQueryable();
        
        if(userId.HasValue)
            clubs = clubs.Where(club => club.ProfessorId == userId.Value || club.ClubStudents.Any(clubStudent => clubStudent.StudentId == userId));
        
        return await clubs.ToListAsync();
    }

    public async Task<Club?> GetClubByIdAsync(Guid id) => await dbContext.Clubs
        .Include(club => club.Professor)
        .Include(club => club.WorkPlans)
        .Include(club => club.Messages).ThenInclude(message => message.Sender)
        .FirstOrDefaultAsync(club => club.Id == id);
    
    public async Task<bool> ClubExistsAsync(Guid clubId) => await dbContext.Clubs.AnyAsync(club => club.Id == clubId);

    public async Task CloseClub(Guid clubId)
    {
        var club = await dbContext.Clubs.FindAsync(clubId);
        if (club == null)
            return;
        
        club.IsActive = false;
        await dbContext.SaveChangesAsync();
    }
}