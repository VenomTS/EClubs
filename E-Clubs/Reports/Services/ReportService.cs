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

public class ReportService(IMapper mapper, ReportRepository reportRepo, ClubRepository clubRepo, WorkPlansRepository workPlansRepo, UserRepository userRepo, AttendanceRepository attendanceRepo)
{
    public async Task<OneOf<IEnumerable<GetReportsResponse>, ClubNotFound>> GetReportsByClubIdAsync(
        GetReportsQueryObject request)
    {
        var clubExists = await clubRepo.ClubExistsAsync(request.ClubId);
        if (!clubExists)
            return new ClubNotFound();
        
        var reports = await reportRepo.GetReportsByClubIdAsync(request.ClubId);

        var reportsDto = mapper.Map<IEnumerable<GetReportsResponse>>(reports).ToList();

        return reportsDto;
    }

    // Mora se pozvati nakon sto je work plan realizovan
    public async Task CreateReportAsync(Guid workPlanId)
    {
        var workPlan = await workPlansRepo.GetWorkPlanByIdAsync(workPlanId);
        if (workPlan == null)
            return;
        
        var club = await clubRepo.GetClubByIdAsync(workPlan.ClubId);
        if (club == null)
            return;

        var professor = await userRepo.GetUserByIdAsync(club.ProfessorId);
        if (professor == null)
            return;
        
        var realizationDate = workPlan.RealizationDate;
        if (realizationDate == null)
            return;
        
        var attendances = await attendanceRepo.GetAttendancesByClubIdByDate(club.Id, realizationDate.Value);

        var presentCount = attendances.Count(x => x.Status == AttendanceStatus.Present);
        var absentCount = attendances.Count(x => x.Status == AttendanceStatus.Absent);
        
        var present = string.Join(";", attendances.Where(x => x.Status == AttendanceStatus.Present).Select(x => x.Student.FirstName + " " + x.Student.LastName));
        var absent = string.Join(";", attendances.Where(x => x.Status == AttendanceStatus.Absent).Select(x => x.Student.FirstName + " " + x.Student.LastName));

        var report = new Report
        {
            ClubId = club.Id,
            WorkPlanId = workPlanId,
            ProfessorId = professor.Id,
            PresentCount = presentCount,
            AbsentCount = absentCount,
            Present = present,
            Absent = absent,
            Date = realizationDate.Value,
        };
        
        await reportRepo.CreateReportAsync(report);
    }
}