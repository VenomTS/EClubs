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

    public async Task<IEnumerable<Club>> GetClubsByUserIdAsync(Guid? userId)
    {
        var clubs = dbContext.Clubs.AsNoTracking()
            .Include(club => club.Professor)
            .AsQueryable();
        
        if(userId.HasValue)
            clubs = clubs.Where(club => club.ProfessorId == userId.Value || club.ClubStudents.Any(clubStudent => clubStudent.StudentId == userId));
        
        return await clubs.ToListAsync();
    }

    public async Task<Club?> GetClubByCodeAsync(string code) =>
        await dbContext.Clubs.FirstOrDefaultAsync(club => club.Code == code);

    public async Task<Club?> GetClubByIdAsync(Guid id) => 
        await dbContext.Clubs.Include(club => club.Professor)
            .FirstOrDefaultAsync(club => club.Id == id);
    
    public async Task<bool> ClubExistsAsync(Guid clubId) => 
        await dbContext.Clubs.AnyAsync(club => club.Id == clubId);
    
    public async Task<bool> CodeExists(string code) => await dbContext.Clubs.AnyAsync(x => x.Code == code && x.IsActive);
}