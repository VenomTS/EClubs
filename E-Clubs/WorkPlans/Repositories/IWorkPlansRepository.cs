using E_Clubs.Database;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.WorkPlans.Repositories;

public interface IWorkPlansRepository
{
    Task<List<WorkPlan>> GetWorkPlansByClubIdAsync(Guid clubId);

    Task<WorkPlan> CreateWorkPlanAsync(WorkPlan workPlan);

    Task RealizeWorkPlanAsync(Guid workPlanId, DateOnly date);

    Task<WorkPlan?> GetCurrentWorkPlanByClubIdAsync(Guid clubId);

    Task<bool> WorkPlanExistsAsync(Guid workPlanId);

    Task<WorkPlan?> GetWorkPlanByIdAsync(Guid requestWorkPlanId);
}