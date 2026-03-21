using AutoMapper;
using E_Clubs.Clubs;
using E_Clubs.Clubs.DTO;
using E_Clubs.Messages;
using E_Clubs.Messages.DTO;
using E_Clubs.Users;
using E_Clubs.Users.DTO;
using E_Clubs.WorkPlans;
using E_Clubs.WorkPlans.DTO;

namespace E_Clubs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, RegisterUserRequest>().ReverseMap();
        CreateMap<User, ClubProfessorResponse>().ReverseMap();
        CreateMap<User, MessageSenderResponse>().ReverseMap();
        
        CreateMap<Club, CreateClubRequest>().ReverseMap();
        CreateMap<Club, CreateClubResponse>().ReverseMap();
        CreateMap<Club, GetClubByIdResponse>().ReverseMap();
        CreateMap<Club, GetAllClubsResponse>().ReverseMap();
        
        CreateMap<WorkPlan, CreateWorkPlanRequest>().ReverseMap();
        CreateMap<WorkPlan, CreateWorkPlanResponse>().ReverseMap();
        CreateMap<WorkPlan, GetAllWorkPlansByClubIdResponse>().ReverseMap();
        CreateMap<WorkPlan, ClubWorkPlansResponse>().ReverseMap();
        
        CreateMap<Message, CreateMessageRequest>().ReverseMap();
        CreateMap<Message, CreateMessageResponse>().ReverseMap();
        CreateMap<Message, GetAllMessagesByClubIdResponse>().ReverseMap();
        CreateMap<Message, ClubMessageResponse>().ReverseMap();
    }
}