using E_Clubs.Database;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.WorkPlans.Repositories;

public class WorkPlansRepository(AppDbContext dbContext) : IWorkPlansRepository
{
    public async Task<List<WorkPlan>> GetWorkPlansByClubIdAsync(Guid clubId)
    {
        var workPlans = await dbContext.WorkPlans.Where(x => x.ClubId == clubId)
            .OrderBy(x => x.Id)
            .ToListAsync();

        return workPlans;
    }

    public async Task<WorkPlan> CreateWorkPlanAsync(WorkPlan workPlan)
    {
        await dbContext.WorkPlans.AddAsync(workPlan);
        await dbContext.SaveChangesAsync();
        return workPlan;
    }

    public async Task RealizeWorkPlanAsync(Guid workPlanId, DateOnly date)
    {
        var workPlan = await dbContext.WorkPlans.FirstOrDefaultAsync(workPlan => workPlan.Id == workPlanId);
        if (workPlan == null)
            return;

        workPlan.RealizationDate = date;
        await dbContext.SaveChangesAsync();
    }

    public async Task<WorkPlan?> GetCurrentWorkPlanByClubIdAsync(Guid clubId) =>
        await dbContext.WorkPlans.Where(x => x.ClubId == clubId && x.RealizationDate == null)
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync();
    
    public async Task<bool> WorkPlanExistsAsync(Guid workPlanId) => 
        await dbContext.WorkPlans.AnyAsync(workPlan => workPlan.Id == workPlanId);

    public async Task<WorkPlan?> GetWorkPlanByIdAsync(Guid requestWorkPlanId) =>
        await dbContext.WorkPlans.FirstOrDefaultAsync(workPlan => workPlan.Id == requestWorkPlanId);
}