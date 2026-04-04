using AutoMapper;
using E_Clubs.Clubs.DTO;
using E_Clubs.Clubs.QueryObjects;
using E_Clubs.Clubs.Repositories;
using E_Clubs.OneOfTypes;
using E_Clubs.Users.Repositories;
using OneOf;
using OneOf.Types;

namespace E_Clubs.Clubs.Services;

public class ClubService(IMapper mapper, ClubRepository clubRepo, ClubStudentRepository clubStudentRepo, UserRepository userRepo)
{
    public async Task<CreateClubResponse> CreateClubAsync(CreateClubRequest request)
    {
        var clubModel = mapper.Map<Club>(request);
        
        clubModel.IsActive = true;
        
        var createdClub = await clubRepo.CreateClubAsync(clubModel);
        
        return mapper.Map<CreateClubResponse>(createdClub);
    }

    public async Task<IEnumerable<GetAllClubsResponse>> GetAllClubsAsync(GetAllClubsQueryObject queryObject)
    {
        var professorClubs = await clubRepo.GetClubsByProfessorIdAsync(queryObject.UserId);
        var studentClubs = await clubRepo.GetClubsByStudentIdAsync(queryObject.UserId);

        var allClubs = professorClubs.Concat(studentClubs);

        var clubsDto = allClubs.Select(mapper.Map<GetAllClubsResponse>);

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
            FirstName = x.Student.FirstName,
            LastName = x.Student.LastName,
        }).ToList();

        return students;
    }

    public async Task<OneOf<GetClubByIdResponse, ClubNotFound>> GetClubByIdAsync(Guid id)
    {
        var club = await clubRepo.GetClubByIdAsync(id);
        
        if(club == null)
            return new ClubNotFound();
        
        var clubDto = mapper.Map<GetClubByIdResponse>(club);

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
}