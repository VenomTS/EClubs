using AutoMapper;
using E_Clubs.Clubs.Repositories;
using E_Clubs.Messages.DTO;
using E_Clubs.Messages.Repositories;
using E_Clubs.OneOfTypes;
using OneOf;

namespace E_Clubs.Messages.Services;

public class MessageService(IMapper mapper, MessageRepository messageRepo, ClubRepository clubRepo)
{
    public async Task<OneOf<List<GetAllMessagesByClubIdResponse>, ClubNotFound>> GetAllMessagesByClubIdAsync(Guid clubId)
    {
        var club = await clubRepo.GetClubByIdAsync(clubId);
        if (club == null)
            return new ClubNotFound();
        
        var messages = await messageRepo.GetMessagesByClubIdAsync(clubId);
        return mapper.Map<List<GetAllMessagesByClubIdResponse>>(messages);
    }
    
    public async Task<OneOf<CreateMessageResponse, ClubNotFound>> CreateMessageAsync(Guid clubId, CreateMessageRequest request)
    {
        var club = await clubRepo.GetClubByIdAsync(clubId);
        if (club == null)
            return new ClubNotFound();
        
        var messageModel = mapper.Map<Message>(request);
        messageModel.ClubId = clubId;

        messageModel = await messageRepo.CreateMessageAsync(messageModel);
        
        var createdMessage = await messageRepo.GetMessageByIdAsync(messageModel.Id);
        return mapper.Map<CreateMessageResponse>(createdMessage);
    }
}