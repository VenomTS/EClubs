using AutoMapper;
using E_Clubs.Attendances.DTO;
using E_Clubs.Attendances.Repositories;
using E_Clubs.Clubs.Repositories;
using E_Clubs.Enums;
using E_Clubs.OneOfTypes;
using E_Clubs.Users.Repositories;
using OneOf;
using OneOf.Types;

namespace E_Clubs.Attendances.Services;

public class AttendanceService(IMapper mapper, AttendanceRepository attendanceRepo, ClubRepository clubRepo, UserRepository userRepo)
{
    public async Task<OneOf<List<GetAllAttendancesResponse>, ClubNotFound>> GetAllAttendancesByClubIdAsync(Guid clubId)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        if(!clubExists)
            return new ClubNotFound();

        var attendances = await attendanceRepo.GetAttendancesByClubId(clubId);
        
        // Can probably be automated using AutoMapper
        var attendancesPerUser = attendances.GroupBy(attendance => attendance.User)
            .Select(user => new GetAllAttendancesResponse
            {
                Student = new AttendanceUserResponse
                {
                    FirstName = user.First().User.FirstName,
                    LastName = user.First().User.LastName,
                },
                AttendanceHistory = user.Select(attendance => new AttendanceHistoryResponse
                {
                    Date = attendance.Date,
                    Status = attendance.Status
                }).ToList()
            });
        
        // return mapper.Map<List<GetAllAttendancesResponse>>(attendances);
        return mapper.Map<List<GetAllAttendancesResponse>>(attendancesPerUser);
    }

    public async Task<OneOf<List<GetUserAttendanceResponse>, ClubNotFound, UserNotFound>>
        GetUserAttendanceByClubIdAsync(Guid clubId, Guid userId)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        if(!clubExists)
            return new ClubNotFound();

        var userExists = await userRepo.UserExistsAsync(userId);
        if (!userExists)
            return new UserNotFound();

        var attendances = await attendanceRepo.GetUserAttendancesByClubId(clubId, userId);
        
        return mapper.Map<List<GetUserAttendanceResponse>>(attendances);
    }

    public async Task<OneOf<Success, ClubNotFound, UserNotFound>> RegisterAttendanceAsync(Guid clubId, RegisterAttendanceRequest request)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        if(!clubExists)
            return new ClubNotFound();
        
        var userExists = await userRepo.UserExistsAsync(request.UserId);
        if (!userExists)
            return new UserNotFound();

        var attendanceModel = mapper.Map<Attendance>(request);

        attendanceModel.ClubId = clubId;
        attendanceModel.Status = AttendanceStatus.Present;

        await attendanceRepo.RegisterAttendance(attendanceModel);
        return new Success();
    }

    public async Task<OneOf<Success, ClubNotFound>> MarkAbsentStudentsAsync(Guid clubId)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        if(!clubExists)
            return new ClubNotFound();
        
        // NOT YET IMPLEMENTED SINCE THERE IS NO JOIN TABLE BETWEEN USERS AND CLUBS
        
        return new Success();
    }
}