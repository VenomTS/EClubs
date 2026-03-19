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
        if(await clubRepo.GetClubByIdAsync(clubId) == null)
            return new ClubNotFound();
        
        var workPlans = await workPlansRepo.GetWorkPlansByClubIdAsync(clubId);

        var workPlansDto = workPlans.Select(mapper.Map<GetAllWorkPlansByClubIdResponse>);
        
        return workPlansDto.ToList();
    }

    public async Task<OneOf<CreateWorkPlanResponse, ClubNotFound, WorkPlanAlreadyExists>> CreateWorkPlanAsync(
        Guid clubId, CreateWorkPlanRequest request)
    {
        var club = await clubRepo.GetClubByIdAsync(clubId);
        if (club == null)
            return new ClubNotFound();

        var workPlanModel = mapper.Map<WorkPlan>(request);
        workPlanModel.ClubId = clubId;
        workPlanModel.Status = WorkPlanStatus.Scheduled;
        workPlanModel.RealizationDate = null;

        var exists = await workPlansRepo.WorkPlanExists(workPlanModel);
        
        if(exists)
            return new WorkPlanAlreadyExists();
        
        var createdWorkPlan = await workPlansRepo.CreateWorkPlanAsync(workPlanModel);

        var workPlanDto = mapper.Map<CreateWorkPlanResponse>(createdWorkPlan);

        return workPlanDto;
    }
}