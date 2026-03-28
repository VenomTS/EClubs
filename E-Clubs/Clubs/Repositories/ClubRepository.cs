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
        
        var createdClub = await GetClubByIdAsync(club.Id);
        return createdClub ?? throw new Exception("Club was not created");
    }

    public async Task<IEnumerable<Club>> GetClubsByProfessorIdAsync(Guid? professorId)
    {
        var allClubs = dbContext.Clubs.AsNoTracking().Include(club => club.Professor).AsQueryable();
        
        if(professorId.HasValue)
            allClubs = allClubs.Where(club => club.ProfessorId == professorId);
        
        return await allClubs.ToListAsync();
    }

    public async Task<IEnumerable<Club>> GetClubsByStudentIdAsync(Guid? studentId)
    {
        var allClubs =
            dbContext.Clubs.Where(club => club.ClubStudents.Any(clubStudent => clubStudent.StudentId == studentId))
                .Include(club => club.Professor);
        
        Console.WriteLine(allClubs.ToQueryString());
        
        return await allClubs.ToListAsync();

    }

    public async Task<Club?> GetClubByIdAsync(Guid id) => await dbContext.Clubs
        .Include(club => club.Professor)
        .Include(club => club.WorkPlans)
        .Include(club => club.Messages)
        .FirstOrDefaultAsync(club => club.Id == id);
    
    public async Task<bool> ClubExistsAsync(Guid clubId) => await dbContext.Clubs.AnyAsync(club => club.Id == clubId);
}