using E_Clubs.Auth.DTO;
using E_Clubs.OneOfTypes;
using OneOf;

namespace E_Clubs.Users.Services;

public interface IUserService
{
    Task<OneOf<GetMeResponse, UserNotFound>> GetMeAsync(string token);
    Task<OneOf<string, UserNotFound>> LoginAsync(LoginUserRequest loginUserRequest);
    Task<OneOf<string, UserAlreadyExists>> RegisterUserAsync(RegisterUserRequest request);
}