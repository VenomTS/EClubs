using AutoMapper;
using E_Clubs.Clubs.Repositories;
using E_Clubs.OneOfTypes;
using E_Clubs.WorkPlans.DTO;
using E_Clubs.WorkPlans.Repositories;
using OneOf;

namespace E_Clubs.WorkPlans.Services;

public class WorkPlansService(IMapper mapper, WorkPlansRepository workPlansRepo, ClubRepository clubRepo)
{
    public async Task<OneOf<List<GetWorkPlanResponse>, ClubNotFound>> GetAllWorkPlansByClubIdAsync(Guid clubId)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        if(!clubExists)
            return new ClubNotFound();
        
        var workPlans = await workPlansRepo.GetWorkPlansByClubIdAsync(clubId);

        var workPlansDto = workPlans.Select(mapper.Map<GetWorkPlanResponse>);
        
        return workPlansDto.ToList();
    }

    public async Task<OneOf<GetWorkPlanResponse, ClubNotFound>> CreateWorkPlanAsync(
        Guid clubId, CreateWorkPlanRequest request)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        if(!clubExists)
            return new ClubNotFound();
        
        var workPlanModel = mapper.Map<WorkPlan>(request);
        workPlanModel.ClubId = clubId;
        workPlanModel.RealizationDate = null;
        
        var createdWorkPlan = await workPlansRepo.CreateWorkPlanAsync(workPlanModel);

        var workPlanDto = mapper.Map<GetWorkPlanResponse>(createdWorkPlan);

        return workPlanDto;
    }

    public async Task<OneOf<GetWorkPlanResponse, ClubNotFound, WorkPlanNotFound>> GetCurrentWorkPlanByClubIdAsync(
        Guid clubId)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        
        if (!clubExists)
            return new ClubNotFound();

        var workPlan = await workPlansRepo.GetCurrentWorkPlanByClubIdAsync(clubId);

        return workPlan == null ? new WorkPlanNotFound() : mapper.Map<GetWorkPlanResponse>(workPlan);
    }

    public async Task<OneOf<IEnumerable<GetDomainsResponse>, ClubNotFound>> GetDomainsByClubIdAsync(Guid clubId)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        
        if(!clubExists)
            return new ClubNotFound();

        var workPlans = await workPlansRepo.GetWorkPlansByClubIdAsync(clubId);

        workPlans = workPlans.DistinctBy(workPlan => workPlan.Domain).ToList();
        
        var domains = mapper.Map<List<GetDomainsResponse>>(workPlans);

        return domains;
    }
}