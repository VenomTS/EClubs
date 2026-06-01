using E_Clubs.Database;
using Microsoft.EntityFrameworkCore;
namespace E_Clubs.Clubs.Repositories;

public interface IClubStudentRepository
{
    Task AddStudentToClub(ClubStudent clubStudent);

    Task DeleteStudentFromClub(ClubStudent clubStudent);

    Task<List<ClubStudent>> GetStudentsByClubIdAsync(Guid clubId);

    Task<bool> IsStudentInClub(Guid clubId, Guid studentId);
}