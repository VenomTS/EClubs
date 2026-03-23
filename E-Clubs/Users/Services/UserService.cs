using AutoMapper;
using E_Clubs.Auth.Services;
using E_Clubs.OneOfTypes;
using E_Clubs.Users.DTO;
using E_Clubs.Users.Repositories;
using OneOf;
using OneOf.Types;

namespace E_Clubs.Users.Services;

public class UserService(IMapper mapper, JWTService jwtService, UserRepository userRepo)
{
    public async Task<OneOf<Success, UserAlreadyExists>> RegisterUserAsync(RegisterUserRequest request)
    {
        if (await userRepo.UserExistsAsync(request.Mail))
            return new UserAlreadyExists();
        
        var userModel = mapper.Map<User>(request);
        
        PasswordHashingService.CreatePasswordHash(request.Password, out var passwordHash, out var passwordSalt);
        
        userModel.PasswordHash = passwordHash;
        userModel.PasswordSalt = passwordSalt;
        await userRepo.CreateUserAsync(userModel);
        return new Success();
    }

    // UserNotFound is used for both invalid mail AND password (for security reasons)
    public async Task<OneOf<LoginUserResponse, UserNotFound>> LoginAsync(LoginUserRequest request)
    {
        var userModel = await userRepo.GetUserByMailAsync(request.Mail);
        if (userModel == null || !PasswordHashingService.VerifyPasswordHash(request.Password, userModel.PasswordHash, userModel.PasswordSalt))
            return new UserNotFound();

        var jwt = jwtService.GenerateToken(userModel);

        var loginUserResponse = new LoginUserResponse
        {
            Token = jwt,
        };

        return loginUserResponse;
    }
}