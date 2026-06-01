using E_Clubs.Auth.DTO;
using E_Clubs.OneOfTypes;
using OneOf;

public interface IUserService
{
    Task<OneOf<GetMeResponse, UserNotFound>> GetMeAsync(string token);
    Task<OneOf<string, UserAlreadyExists>> LoginAsync(LoginUserRequest loginUserRequest);
    Task<OneOf<string, UserAlreadyExists>> RegisterUserAsync(RegisterUserRequest request);
}