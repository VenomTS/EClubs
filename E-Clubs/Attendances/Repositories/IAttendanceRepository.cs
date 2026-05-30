namespace E_Clubs.Attendances.Repositories;

public interface IAttendanceRepository
{
    Task<List<Attendance>> GetAttendancesByClubId(Guid clubId);
    Task<List<Attendance>> GetUserAttendancesByClubId(Guid clubId, Guid userId);
    Task RegisterAttendance(Attendance attendance);
    Task<List<Attendance>> GetAttendancesByClubIdByDate(Guid clubId, DateOnly date);
}