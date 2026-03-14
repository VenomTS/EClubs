using AutoMapper;
using E_Clubs.DTO.UserDTO;
using E_Clubs.Models;

namespace E_Clubs.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, RegisterUserRequest>().ReverseMap();
    }
}