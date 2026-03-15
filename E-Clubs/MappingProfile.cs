using AutoMapper;
using E_Clubs.Clubs;
using E_Clubs.Clubs.DTO;
using E_Clubs.Users;
using E_Clubs.Users.DTO;

namespace E_Clubs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, RegisterUserRequest>().ReverseMap();
        CreateMap<User, ClubProfessorResponse>().ReverseMap();
        
        CreateMap<Club, CreateClubRequest>().ReverseMap();
        CreateMap<Club, CreateClubResponse>().ReverseMap();
        CreateMap<Club, GetClubByIdResponse>().ReverseMap();
        CreateMap<Club, GetAllClubsResponse>().ReverseMap();
    }
}