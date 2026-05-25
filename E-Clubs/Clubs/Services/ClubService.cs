using System.Text;
using AutoMapper;
using E_Clubs.Clubs.DTO;
using E_Clubs.Clubs.QueryObjects;
using E_Clubs.Clubs.Repositories;
using E_Clubs.OneOfTypes;
using E_Clubs.Users.DTO;
using E_Clubs.Users.Repositories;
using OneOf;
using OneOf.Types;

namespace E_Clubs.Clubs.Services;

public class ClubService(IMapper mapper, ClubRepository clubRepo, ClubStudentRepository clubStudentRepo, UserRepository userRepo)
{
    private const string CodeCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int CodeLength = 6;
    
    public async Task<GetClubResponse> CreateClubAsync(CreateClubRequest request)
    {
        var clubModel = mapper.Map<Club>(request);
        
        clubModel.IsActive = true;
        clubModel.Code = await GenerateClubCode();
        
        var createdClub = await clubRepo.CreateClubAsync(clubModel);
        
        return mapper.Map<GetClubResponse>(createdClub);
    }

    public async Task<IEnumerable<GetClubResponse>> GetClubsByUserIdAsync(GetClubsQueryObject queryObject)
    {
        var clubs = await clubRepo.GetClubsByUserIdAsync(queryObject.UserId);

        var clubsDto = mapper.Map<List<GetClubResponse>>(clubs);

        return clubsDto;
    }

    public async Task<OneOf<IEnumerable<GetUserResponse>, ClubNotFound>> GetStudentsByClubIdAsync(Guid clubId)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        if (!clubExists)
            return new ClubNotFound();

        var clubStudents = await clubStudentRepo.GetStudentsByClubIdAsync(clubId);

        var students = clubStudents.Select(x => new GetUserResponse
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

    public async Task<OneOf<Success, ClubNotFound, StudentNotFound, StudentAlreadyInClub>> AddStudentToClubAsync(JoinClubRequest request)
    {
        var club = await clubRepo.GetClubByCodeAsync(request.Code);

        if (club == null)
            return new ClubNotFound();

        // Dodatni check da se provjeri je li actually student
        var studentExists = await userRepo.UserExistsAsync(request.StudentId);
        if (!studentExists)
            return new StudentNotFound();
        
        var isStudentInClub = await clubStudentRepo.IsStudentInClub(club.Id, request.StudentId);
        if (isStudentInClub)
            return new StudentAlreadyInClub();

        var clubStudent = new ClubStudent
        {
            ClubId = club.Id,
            StudentId = request.StudentId,
            Club = null!,
            Student = null!,
        };

        await clubStudentRepo.AddStudentToClub(clubStudent);
        return new Success();
    }

    public async Task<OneOf<Success, ClubNotFound, StudentNotFound, ClubStudentNotFound>> DeleteStudentFromClub(
        Guid clubId, KickStudentRequest request)
    {
        var clubExists = await clubRepo.ClubExistsAsync(clubId);
        if (!clubExists)
            return new ClubNotFound();

        var studentExists = await userRepo.UserExistsAsync(request.StudentId);
        if (!studentExists)
            return new StudentNotFound();

        var isStudentInClub = await clubStudentRepo.IsStudentInClub(clubId, request.StudentId);
        if (!isStudentInClub)
            return new ClubStudentNotFound();

        var clubStudent = new ClubStudent
        {
            ClubId = clubId,
            StudentId = request.StudentId,
            Club = null!,
            Student = null!,
        };
        
        await clubStudentRepo.DeleteStudentFromClub(clubStudent);
        return new Success();
    }

    private async Task<string> GenerateClubCode()
    {
        var code = new StringBuilder();
        do
        {
            code.Clear();
            var random = new Random();
            while(code.Length < CodeLength)
                code.Append(CodeCharacters[random.Next(CodeCharacters.Length)]);
        } while (await clubRepo.CodeExists(code.ToString()));

        return code.ToString();
    }
}