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

public interface IClubService
{

    Task<GetClubResponse> CreateClubAsync(CreateClubRequest request);


    Task<IEnumerable<GetClubResponse>> GetClubsByUserIdAsync(GetClubsQueryObject queryObject);


    Task<OneOf<IEnumerable<GetUserResponse>, ClubNotFound>> GetStudentsByClubIdAsync(Guid clubId);

    Task<OneOf<GetClubResponse, ClubNotFound>> GetClubByIdAsync(Guid id);

    Task<OneOf<Success, ClubNotFound, StudentNotFound, StudentAlreadyInClub>> AddStudentToClubAsync(JoinClubRequest request);



    Task<OneOf<Success, ClubNotFound, StudentNotFound, ClubStudentNotFound>> DeleteStudentFromClub(
        Guid clubId, KickStudentRequest request);
}