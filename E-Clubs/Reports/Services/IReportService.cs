using AutoMapper;
using E_Clubs.Attendances.Repositories;
using E_Clubs.Clubs.Repositories;
using E_Clubs.Enums;
using E_Clubs.OneOfTypes;
using E_Clubs.Reports.DTO;
using E_Clubs.Reports.QueryObject;
using E_Clubs.Reports.Repositories;
using E_Clubs.Users.Repositories;
using E_Clubs.WorkPlans.Repositories;
using OneOf;

namespace E_Clubs.Reports.Services;

public interface IReportService
{
    Task<OneOf<IEnumerable<GetReportsResponse>, ClubNotFound>> GetReportsByClubIdAsync(
        GetReportsQueryObject request);

    Task CreateReportAsync(Guid workPlanId);
}