using AutoMapper;
using E_Clubs.DTO.UserDTO;
using E_Clubs.Models;
using E_Clubs.OneOfTypes;
using E_Clubs.Repositories;
using OneOf;
using OneOf.Types;

namespace E_Clubs.Services;

public class UserService(IMapper mapper, PasswordHashingService hashService, JWTService jwtService, UserRepository userRepo, UserRoleRepository userRoleRepo)
{
    public async Task<OneOf<Success, UserAlreadyExists>> RegisterUserAsync(RegisterUserRequest request)
    {
        if (await userRepo.UserExistsAsync(request.Mail))
            return new UserAlreadyExists();
        
        var userModel = mapper.Map<User>(request);
        
        hashService.CreatePasswordHash(request.Password, out var passwordHash, out var passwordSalt);
        
        userModel.PasswordHash = passwordHash;
        userModel.PasswordSalt = passwordSalt;
        await userRepo.CreateUserAsync(userModel);
        return new Success();
    }

    // UserNotFound is used for both invalid mail AND password (for security reasons)
    public async Task<OneOf<string, UserNotFound>> LoginAsync(LoginUserRequest request)
    {
        var userModel = await userRepo.GetUserByMailAsync(request.Mail);
        if (userModel == null)
            return new UserNotFound();

        if (!hashService.VerifyPasswordHash(request.Password, userModel.PasswordHash, userModel.PasswordSalt))
            return new UserNotFound();

        var userRoles = await userRoleRepo.GetUserRolesAsync(userModel.Id);

        var jwt = jwtService.GenerateToken(userModel, userRoles);

        return jwt;
    }
}