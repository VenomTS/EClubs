using E_Clubs.Database;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.Attendances.Repositories;

public class AttendanceRepository(AppDbContext dbContext)
{

    public async Task<List<Attendance>> GetAttendancesByClubId(Guid clubId)
    {
        var attendances = await dbContext.Attendances.Where(attendance => attendance.ClubId == clubId)
            .Include(attendance => attendance.Student)
            .ToListAsync();
        
        return attendances;
    }

    public async Task<List<Attendance>> GetUserAttendancesByClubId(Guid clubId, Guid userId)
    {
        var attendances = await dbContext.Attendances.Where(attendance => attendance.ClubId == clubId && attendance.StudentId == userId).ToListAsync();
        
        return attendances;
    }

    public async Task RegisterAttendance(Attendance attendance)
    {
        await dbContext.Attendances.AddAsync(attendance);
        await dbContext.SaveChangesAsync();
    }
    
}