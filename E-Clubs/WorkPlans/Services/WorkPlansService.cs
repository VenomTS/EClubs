using AutoMapper;
using E_Clubs.Clubs.Repositories;
using E_Clubs.Enums;
using E_Clubs.OneOfTypes;
using E_Clubs.WorkPlans.DTO;
using E_Clubs.WorkPlans.Repositories;
using OneOf;

namespace E_Clubs.WorkPlans.Services;

public class WorkPlansService(IMapper mapper, WorkPlansRepository workPlansRepo, ClubRepository clubRepo)
{
    public async Task<OneOf<List<GetAllWorkPlansByClubIdResponse>, ClubNotFound>> GetAllWorkPlansByClubIdAsync(Guid clubId)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        if(!clubExists)
            return new ClubNotFound();
        
        var workPlans = await workPlansRepo.GetWorkPlansByClubIdAsync(clubId);

        var workPlansDto = workPlans.Select(mapper.Map<GetAllWorkPlansByClubIdResponse>);
        
        return workPlansDto.ToList();
    }

    public async Task<OneOf<CreateWorkPlanResponse, ClubNotFound>> CreateWorkPlanAsync(
        Guid clubId, CreateWorkPlanRequest request)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        if(!clubExists)
            return new ClubNotFound();

        var existingWorkPlanCount = await workPlansRepo.GetWorkPlanCountByClubIdAsync(clubId);
        
        var workPlanModel = mapper.Map<WorkPlan>(request);
        workPlanModel.ClubId = clubId;
        workPlanModel.Status = WorkPlanStatus.Scheduled;
        workPlanModel.RealizationDate = null;
        workPlanModel.LessonNumber = existingWorkPlanCount + 1;
        
        var createdWorkPlan = await workPlansRepo.CreateWorkPlanAsync(workPlanModel);

        var workPlanDto = mapper.Map<CreateWorkPlanResponse>(createdWorkPlan);

        return workPlanDto;
    }

    public async Task<OneOf<GetCurrentWorkPlanResponse, ClubNotFound, WorkPlanNotFound>> GetCurrentWorkPlanByClubIdAsync(
        Guid clubId)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        
        if (!clubExists)
            return new ClubNotFound();

        var workPlan = await workPlansRepo.GetCurrentWorkPlanByClubIdAsync(clubId);

        return workPlan == null ? new WorkPlanNotFound() : mapper.Map<GetCurrentWorkPlanResponse>(workPlan);
    }

    public async Task<OneOf<IEnumerable<GetDomainsResponse>, ClubNotFound>> GetDomainsByClubIdAsync(Guid clubId)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        
        if(!clubExists)
            return new ClubNotFound();

        var workPlans = await workPlansRepo.GetWorkPlansByClubIdAsync(clubId);

        workPlans = workPlans.DistinctBy(workPlan => workPlan.Domain).ToList();

        var domains = workPlans.Select(x => new GetDomainsResponse
        {
            DomainNumber = x.DomainNumber,
            Domain = x.Domain,
        }).ToList();

        return domains;
    }
}