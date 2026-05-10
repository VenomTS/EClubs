using AutoMapper;
using E_Clubs.Attendances.DTO;
using E_Clubs.Attendances.Repositories;
using E_Clubs.Clubs.Repositories;
using E_Clubs.Enums;
using E_Clubs.OneOfTypes;
using E_Clubs.Users.DTO;
using E_Clubs.Users.Repositories;
using E_Clubs.WorkPlans.Repositories;
using OneOf;
using OneOf.Types;

namespace E_Clubs.Attendances.Services;

public class AttendanceService(IMapper mapper, AttendanceRepository attendanceRepo, ClubRepository clubRepo, UserRepository userRepo, WorkPlansRepository workPlansRepo, ClubStudentRepository clubStudentRepo)
{
    public async Task<OneOf<List<GetAttendanceResponse>, ClubNotFound>> GetAllAttendancesByClubIdAsync(Guid clubId)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        if(!clubExists)
            return new ClubNotFound();

        var attendances = await attendanceRepo.GetAttendancesByClubId(clubId);
        
        // Can probably be automated using AutoMapper
        var attendancesPerUser = attendances.GroupBy(attendance => attendance.Student)
            .Select(user => new GetAttendanceResponse
            {
                Student = new GetUserResponse
                {
                    Id = user.First().Student.Id,
                    FirstName = user.First().Student.FirstName,
                    LastName = user.First().Student.LastName,
                },
                AttendanceHistory = user.Select(attendance => new AttendanceHistoryResponse
                {
                    Date = attendance.Date,
                    Status = attendance.Status
                }).ToList()
            });
        
        // return mapper.Map<List<GetAllAttendancesResponse>>(attendances);
        return mapper.Map<List<GetAttendanceResponse>>(attendancesPerUser);
    }

    public async Task<OneOf<GetAttendanceResponse, ClubNotFound, UserNotFound>>
        GetUserAttendanceByClubIdAsync(Guid clubId, Guid userId)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        if(!clubExists)
            return new ClubNotFound();
        
        var user = await userRepo.GetUserByIdAsync(userId);
        if (user == null)
            return new UserNotFound();

        var attendances = await attendanceRepo.GetUserAttendancesByClubId(clubId, userId);

        var attendancesDto = new GetAttendanceResponse
        {
            Student = new GetUserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
            },
            AttendanceHistory = attendances.Select(x => new AttendanceHistoryResponse
            {
                Date = x.Date,
                Status = x.Status,
            }).ToList()
        };

        return attendancesDto;
    }

    public async Task<OneOf<Success, ClubNotFound, UserNotFound>> RegisterAttendanceAsync(Guid clubId, RegisterAttendanceRequest request)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        if(!clubExists)
            return new ClubNotFound();
        
        var userExists = await userRepo.UserExistsAsync(request.StudentId);
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

        var currentWorkPlan = await workPlansRepo.GetCurrentWorkPlanByClubIdAsync(clubId);
        if (currentWorkPlan == null)
            throw new Exception("Current Work Plan is Null but attendance is being taken");

        var students = await clubStudentRepo.GetStudentsByClubIdAsync(clubId);

        foreach (var student in students)
        {
            var wasPresent = await attendanceRepo.WasUserPresentByWorkPlanId(student.StudentId, currentWorkPlan.Id);
            if (wasPresent)
                continue;

            var attendance = new Attendance
            {
                ClubId = clubId,
                WorkPlanId = currentWorkPlan.Id,
                StudentId = student.StudentId,
                Status = AttendanceStatus.Absent,
                Club = null!,
                WorkPlan = null!,
                Student = null!
            };

            await attendanceRepo.RegisterAttendance(attendance);
        }
        
        return new Success();
    }
}