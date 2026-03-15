using AutoMapper;
using E_Clubs.DTO.ClubDTO;
using E_Clubs.Models;
using E_Clubs.OneOfTypes;
using E_Clubs.QueryObjects;
using E_Clubs.Repositories;
using OneOf;

namespace E_Clubs.Services;

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
        var clubs = await clubRepo.GetAllClubsAsync(queryObject);

        var clubsDto = clubs.Select(mapper.Map<GetAllClubsResponse>);

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