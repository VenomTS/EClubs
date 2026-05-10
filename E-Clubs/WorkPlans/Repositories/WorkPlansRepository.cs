using E_Clubs.Database;
using E_Clubs.Enums;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.WorkPlans.Repositories;

public class WorkPlansRepository(AppDbContext dbContext)
{
    public async Task<List<WorkPlan>> GetWorkPlansByClubIdAsync(Guid clubId)
    {
        var workPlans = await dbContext.WorkPlans.Where(x => x.ClubId == clubId).ToListAsync();

        return workPlans;
    }

    public async Task<WorkPlan> CreateWorkPlanAsync(WorkPlan workPlan)
    {
        await dbContext.WorkPlans.AddAsync(workPlan);
        await dbContext.SaveChangesAsync();
        return workPlan;
    }

    public async Task<int> GetWorkPlanCountByClubIdAsync(Guid clubId)
    {
        return await dbContext.WorkPlans.Where(x => x.ClubId == clubId).CountAsync();
    }

    public async Task<WorkPlan?> GetCurrentWorkPlanByClubIdAsync(Guid clubId)
    {
        return await dbContext.WorkPlans.Where(x => x.ClubId == clubId && x.RealizationDate == null).FirstOrDefaultAsync();
    }
    
    public async Task<bool> WorkPlanExistsAsync(Guid workPlanId) => await dbContext.WorkPlans.AnyAsync(workPlan => workPlan.Id == workPlanId);

    public async Task ConcludeWorkPlanAsync(Guid workPlanId)
    {
        var workPlan = await dbContext.WorkPlans.FindAsync(workPlanId);
        if (workPlan == null)
            return;

        workPlan.RealizationDate = new DateOnly().AddDays(0);
        workPlan.Status = WorkPlanStatus.Completed;
        await dbContext.SaveChangesAsync();
    }
}