using E_Clubs.Database;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.Reports.Repositories;

public class ReportRepository(AppDbContext context)
{
    public async Task<IEnumerable<Report>> GetReportsByClubIdAsync(Guid clubId)
    {
        return await context.Reports.Where(x => x.ClubId == clubId)
            .Include(x => x.Club)
            .Include(x => x.WorkPlan)
            .Include(x => x.Professor)
            .ToListAsync();
    }

    public async Task<Report> CreateReportAsync(Report report)
    {
        await context.Reports.AddAsync(report);
        await context.SaveChangesAsync();

        return report;
    }
}