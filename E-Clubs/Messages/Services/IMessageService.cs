using AutoMapper;
using E_Clubs.Clubs.Repositories;
using E_Clubs.Messages.DTO;
using E_Clubs.Messages.Repositories;
using E_Clubs.OneOfTypes;
using OneOf;
using OneOf.Types;

namespace E_Clubs.Messages.Services;

public interface IMessageService
{Task<OneOf<GetMessageResponse, MessageNotFound>> GetMessageByIdAsync(Guid messageId);
Task<OneOf<List<GetMessageResponse>, ClubNotFound>> GetAllMessagesByClubIdAsync(Guid clubId);
 Task<OneOf<GetMessageResponse, ClubNotFound>> CreateMessageAsync(Guid clubId, CreateMessageRequest request);
Task<OneOf<Success, MessageNotFound>> DeleteMessageByIdAsync(Guid messageId);
    Task<OneOf<Success, MessageNotFound>> UpdateMessageByIdAsync(Guid messageId,
        UpdateMessageRequest request);
}