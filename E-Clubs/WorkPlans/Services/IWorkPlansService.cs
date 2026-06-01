using System.Diagnostics;
using AutoMapper;
using E_Clubs.Attendances;
using E_Clubs.Attendances.Repositories;
using E_Clubs.Clubs.Repositories;
using E_Clubs.Enums;
using E_Clubs.OneOfTypes;
using E_Clubs.Reports.Services;
using E_Clubs.WorkPlans.DTO;
using E_Clubs.WorkPlans.Repositories;
using OneOf;
using OneOf.Types;

namespace E_Clubs.WorkPlans.Services;

public interface IWorkPlansService
{
    Task<OneOf<List<GetWorkPlanResponse>, ClubNotFound>> GetAllWorkPlansByClubIdAsync(Guid clubId);

    Task<OneOf<GetWorkPlanResponse, ClubNotFound>> CreateWorkPlanAsync(
        Guid clubId, CreateWorkPlanRequest request);

    Task<OneOf<GetWorkPlanResponse, ClubNotFound, WorkPlanNotFound>> GetCurrentWorkPlanByClubIdAsync(
        Guid clubId);

    Task<OneOf<IEnumerable<GetDomainsResponse>, ClubNotFound>> GetDomainsByClubIdAsync(Guid clubId);

    Task<OneOf<Success, ClubNotFound, InvalidFile>> UploadWorkPlans(Guid clubId, IFormFile file);
    Task<OneOf<Success, ClubNotFound, WorkPlanNotFound>> ConcludeWorkPlan(Guid clubId, ConcludeWorkPlanRequest request);
}
