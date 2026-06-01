using E_Clubs.Attendances.DTO;
using E_Clubs.OneOfTypes;
using OneOf;
using OneOf.Types;

namespace E_Clubs.Attendances.Services;

public interface IAttendanceService
{
    Task<OneOf<List<GetAttendanceResponse>, ClubNotFound>> GetAllAttendancesByClubIdAsync(Guid clubId);
    Task<OneOf<GetAttendanceResponse, ClubNotFound, UserNotFound>> GetUserAttendanceByClubIdAsync(Guid clubId, Guid userId);
    Task<OneOf<Success, ClubNotFound, UserNotFound>> RegisterAttendanceAsync(Guid clubId, RegisterAttendanceRequest request);
}