using AutoMapper;
using E_Clubs.Clubs.DTO;
using E_Clubs.Clubs.QueryObjects;
using E_Clubs.Clubs.Repositories;
using E_Clubs.OneOfTypes;
using E_Clubs.Users.Repositories;
using E_Clubs.WorkPlans.Repositories;
using OneOf;
using OneOf.Types;

namespace E_Clubs.Clubs.Services;

public class ClubService(IMapper mapper, ClubRepository clubRepo, ClubStudentRepository clubStudentRepo, UserRepository userRepo, WorkPlansRepository workPlansRepo)
{
    public async Task<GetClubResponse> CreateClubAsync(CreateClubRequest request)
    {
        var clubModel = mapper.Map<Club>(request);
        
        clubModel.IsActive = true;
        
        var createdClub = await clubRepo.CreateClubAsync(clubModel);
        
        return mapper.Map<GetClubResponse>(createdClub);
    }

    public async Task<IEnumerable<GetClubResponse>> GetClubsByUserIdAsync(GetClubsQueryObject queryObject)
    {
        var clubs = await clubRepo.GetClubsByUserIdAsync(queryObject.UserId);

        var clubsDto = mapper.Map<List<GetClubResponse>>(clubs);

        return clubsDto;
    }

    public async Task<OneOf<IEnumerable<GetStudentByClubIdResponse>, ClubNotFound>> GetStudentsByClubIdAsync(Guid clubId)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        if (!clubExists)
            return new ClubNotFound();

        var clubStudents = await clubStudentRepo.GetStudentsByClubIdAsync(clubId);

        var students = clubStudents.Select(x => new GetStudentByClubIdResponse
        {
            Id = x.StudentId,
            FirstName = x.Student.FirstName,
            LastName = x.Student.LastName,
        }).ToList();

        return students;
    }

    public async Task<OneOf<GetClubResponse, ClubNotFound>> GetClubByIdAsync(Guid id)
    {
        var club = await clubRepo.GetClubByIdAsync(id);
        
        if(club == null)
            return new ClubNotFound();
        
        var clubDto = mapper.Map<GetClubResponse>(club);

        return clubDto;
    }

    public async Task<OneOf<Success, ClubNotFound, StudentNotFound, StudentAlreadyInClub>> AddStudentToClubAsync(Guid clubId,
        Guid studentId)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        if (!clubExists)
            return new ClubNotFound();

        // Dodatni check da se provjeri je li actually student
        var studentExists = await userRepo.UserExistsAsync(studentId);
        if (!studentExists)
            return new StudentNotFound();
        
        var isStudentInClub = await clubStudentRepo.IsStudentInClub(clubId, studentId);
        if (isStudentInClub)
            return new StudentAlreadyInClub();

        var clubStudent = new ClubStudent
        {
            ClubId = clubId,
            StudentId = studentId,
            Club = null!,
            Student = null!,
        };

        await clubStudentRepo.AddStudentToClub(clubStudent);
        return new Success();
    }

    public async Task<OneOf<Success, ClubNotFound, StudentNotFound, ClubStudentNotFound>> DeleteStudentFromClub(
        Guid clubId, Guid studentId)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        if (!clubExists)
            return new ClubNotFound();

        var studentExists = await userRepo.UserExistsAsync(studentId);
        if (!studentExists)
            return new StudentNotFound();

        var isStudentInClub = await clubStudentRepo.IsStudentInClub(clubId, studentId);
        if (!isStudentInClub)
            return new ClubStudentNotFound();

        var clubStudent = new ClubStudent
        {
            ClubId = clubId,
            StudentId = studentId,
            Club = null!,
            Student = null!,
        };
        
        await clubStudentRepo.DeleteStudentFromClub(clubStudent);
        return new Success();
    }

    public async Task ConcludeWorkPlan(Guid clubId)
    {
        var club = await clubRepo.GetClubByIdAsync(clubId);
        if (club == null)
            throw new Exception("Club does not exist but workplan does");

        var currentWorkPlan = await workPlansRepo.GetCurrentWorkPlanByClubIdAsync(clubId);
        if (currentWorkPlan == null)
            throw new Exception("Concluding non existential work plan");

        await workPlansRepo.ConcludeWorkPlanAsync(currentWorkPlan.Id);
        
        currentWorkPlan = await workPlansRepo.GetCurrentWorkPlanByClubIdAsync(clubId);
        if (currentWorkPlan != null)
            return;

        await clubRepo.CloseClub(club.Id);
    }
}