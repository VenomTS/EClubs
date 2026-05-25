using System.Diagnostics;
using AutoMapper;
using E_Clubs.Attendances;
using E_Clubs.Attendances.Repositories;
using E_Clubs.Clubs.Repositories;
using E_Clubs.Enums;
using E_Clubs.Messages.DTO;
using E_Clubs.OneOfTypes;
using E_Clubs.WorkPlans.DTO;
using E_Clubs.WorkPlans.Repositories;
using OneOf;
using OneOf.Types;

namespace E_Clubs.WorkPlans.Services;

public class WorkPlansService(IMapper mapper, WorkPlansRepository workPlansRepo, ClubRepository clubRepo, ClubStudentRepository clubStudentRepo, AttendanceRepository attendanceRepo)
{
    public async Task<OneOf<List<GetWorkPlanResponse>, ClubNotFound>> GetAllWorkPlansByClubIdAsync(Guid clubId)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        if(!clubExists)
            return new ClubNotFound();
        
        var workPlans = await workPlansRepo.GetWorkPlansByClubIdAsync(clubId);

        var workPlansDto = workPlans.Select(mapper.Map<GetWorkPlanResponse>);
        
        return workPlansDto.ToList();
    }

    public async Task<OneOf<GetWorkPlanResponse, ClubNotFound>> CreateWorkPlanAsync(
        Guid clubId, CreateWorkPlanRequest request)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        if(!clubExists)
            return new ClubNotFound();
        
        var workPlanModel = mapper.Map<WorkPlan>(request);
        workPlanModel.ClubId = clubId;
        workPlanModel.RealizationDate = null;
        
        var createdWorkPlan = await workPlansRepo.CreateWorkPlanAsync(workPlanModel);

        var workPlanDto = mapper.Map<GetWorkPlanResponse>(createdWorkPlan);

        return workPlanDto;
    }

    public async Task<OneOf<GetWorkPlanResponse, ClubNotFound, WorkPlanNotFound>> GetCurrentWorkPlanByClubIdAsync(
        Guid clubId)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        
        if (!clubExists)
            return new ClubNotFound();

        var workPlan = await workPlansRepo.GetCurrentWorkPlanByClubIdAsync(clubId);

        return workPlan == null ? new WorkPlanNotFound() : mapper.Map<GetWorkPlanResponse>(workPlan);
    }

    public async Task<OneOf<IEnumerable<GetDomainsResponse>, ClubNotFound>> GetDomainsByClubIdAsync(Guid clubId)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        
        if(!clubExists)
            return new ClubNotFound();

        var workPlans = await workPlansRepo.GetWorkPlansByClubIdAsync(clubId);

        workPlans = workPlans.DistinctBy(workPlan => workPlan.Domain).ToList();
        
        var domains = mapper.Map<List<GetDomainsResponse>>(workPlans);

        return domains;
    }

    public async Task<OneOf<Success, ClubNotFound, InvalidFile>> UploadWorkPlans(Guid clubId, IFormFile file)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        
        if(!clubExists)
            return new ClubNotFound();
        
        
        if(file.Length == 0)
            return new InvalidFile();

        var tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");

        await using var stream = new FileStream(tempFilePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var psi = new ProcessStartInfo
        {
            FileName = @"C:\Users\Tarik\PycharmProjects\ExcelParser\.venv\Scripts\python.exe",
            Arguments = $@"C:\Users\Tarik\RiderProjects\EClubs\E-Clubs\External\excelParser.py -i ""{tempFilePath}"" -cId {clubId}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        
        using var process = Process.Start(psi);
        if (process == null)
            return new InvalidFile();
        
        await process.WaitForExitAsync();

        File.Delete(tempFilePath);

        if (process.ExitCode != 0)
            return new InvalidFile();

        return new Success();
    }    public async Task<OneOf<Success, ClubNotFound, WorkPlanNotFound>> ConcludeWorkPlan(Guid clubId, ConcludeWorkPlanRequest request)
    {
        var club = await clubRepo.ClubExistsAsync(clubId);
        if (!club)
            return new ClubNotFound();

        var workPlan = await workPlansRepo.GetWorkPlanByIdAsync(request.WorkPlanId);
        if (workPlan == null)
            return new WorkPlanNotFound();

        var students = await clubStudentRepo.GetStudentsByClubIdAsync(clubId);

        var attendances = await attendanceRepo.GetAttendancesByClubIdByDate(clubId, request.Date);
        
        // Uzmemo sve studente koji su dio sekcije i sve one koji su bili prisutni
        // Prodjemo kroz loop ako je student bio prisutan preskocimo ga ako nije oznacimo da je bio odsutan

        foreach (var student in students)
        {
            if (attendances.Any(x => x.StudentId == student.StudentId))
                continue;

            await attendanceRepo.RegisterAttendance(new Attendance
            {
                ClubId = clubId,
                StudentId = student.StudentId,
                Date = request.Date,
                Status = AttendanceStatus.Absent,
                Club = null!,
                Student = null!,
            });
        }
        
        await workPlansRepo.RealizeWorkPlanAsync(request.WorkPlanId, request.Date);
        return new Success();
    }
}
