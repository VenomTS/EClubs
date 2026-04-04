using E_Clubs.Database;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.Clubs.Repositories;

public class ClubStudentRepository(AppDbContext dbContext)
{
    public async Task AddStudentToClub(ClubStudent clubStudent)
    {
        await dbContext.ClubStudents.AddAsync(clubStudent);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteStudentFromClub(ClubStudent clubStudent)
    {
        dbContext.ClubStudents.Remove(clubStudent);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<ClubStudent>> GetStudentsByClubIdAsync(Guid clubId)
    {
        var students = await dbContext.ClubStudents
            .Where(clubStudent => clubStudent.ClubId == clubId)
            .Include(x => x.Student)
            .ToListAsync();

        return students;
    }

    public async Task<bool> IsStudentInClub(Guid clubId, Guid studentId)
    {
        return await dbContext.ClubStudents.AnyAsync(x => x.ClubId == clubId && x.StudentId == studentId);
    }
}