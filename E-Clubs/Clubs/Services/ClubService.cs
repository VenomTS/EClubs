using AutoMapper;
using E_Clubs.Clubs.DTO;
using E_Clubs.Clubs.QueryObjects;
using E_Clubs.Clubs.Repositories;
using E_Clubs.OneOfTypes;
using OneOf;

namespace E_Clubs.Clubs.Services;

public class ClubService(IMapper mapper, ClubRepository clubRepo)
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

    public async Task<OneOf<GetClubByIdResponse, ClubNotFound>> GetClubByIdAsync(Guid id)
    {
        var club = await clubRepo.GetClubByIdAsync(id);
        
        if(club == null)
            return new ClubNotFound();
        
        var clubDto = mapper.Map<GetClubByIdResponse>(club);

        return clubDto;
    }
}