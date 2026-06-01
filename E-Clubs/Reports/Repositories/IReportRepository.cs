using E_Clubs.Database;
using Microsoft.EntityFrameworkCore;

namespace E_Clubs.Reports.Repositories;

public interface IReportRepository
{
    Task<IEnumerable<Report>> GetReportsByClubIdAsync(Guid clubId);
    Task<Report> CreateReportAsync(Report report);
}