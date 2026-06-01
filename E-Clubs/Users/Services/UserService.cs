using AutoMapper;
using E_Clubs.Auth.DTO;
using E_Clubs.Auth.Services;
using E_Clubs.Enums;
using E_Clubs.OneOfTypes;
using E_Clubs.Users.Repositories;
using OneOf;

namespace E_Clubs.Users.Services;

public class UserService(IMapper mapper, JWTService jwtService, IUserRepository userRepo) : IUserService
{
    public async Task<OneOf<GetMeResponse, UserNotFound>> GetMeAsync(string token)
    {
        var userId = JWTService.GetIdFromToken(token);
        
        var userIdGuid = new Guid(userId);

        var user = await userRepo.GetUserByIdAsync(userIdGuid);
        
        if(user == null)
            return new UserNotFound();

        return new GetMeResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = RolesToList(user.Roles),
        };
    }

    private static List<string> RolesToList(Roles roles)
    {
        var result = new List<string>();

        foreach (var role in Enum.GetValues<Roles>())
        {
            if (!roles.HasFlag(role))
                continue;
            
            result.Add(role.ToString());
        }

        return result;
    }
    
    public async Task<OneOf<string, UserAlreadyExists>> RegisterUserAsync(RegisterUserRequest request)
    {
        if (await userRepo.UserExistsAsync(request.Mail))
            return new UserAlreadyExists();
        
        var userModel = mapper.Map<User>(request);
        
        PasswordHashingService.CreatePasswordHash(request.Password, out var passwordHash, out var passwordSalt);
        
        // Initialize default role as a student
        userModel.PasswordHash = passwordHash;
        userModel.PasswordSalt = passwordSalt;
        userModel.Roles = request.IsStudent ? Roles.Student : Roles.Professor;
        await userRepo.CreateUserAsync(userModel);
        
        var token = jwtService.GenerateToken(userModel);

        return token;
    }

    // UserNotFound is used for both invalid mail AND password (for security reasons)
    public async Task<OneOf<string, UserNotFound>> LoginAsync(LoginUserRequest request)
    {
        var userModel = await userRepo.GetUserByMailAsync(request.Mail);
        if (userModel == null || !PasswordHashingService.VerifyPasswordHash(request.Password, userModel.PasswordHash, userModel.PasswordSalt))
            return new UserNotFound();

        var jwt = jwtService.GenerateToken(userModel);

        return jwt;
    }
}