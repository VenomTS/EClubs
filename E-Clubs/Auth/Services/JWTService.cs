using System.Security.Claims;
using System.Text;
using E_Clubs.Enums;
using E_Clubs.Users;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace E_Clubs.Auth.Services;

public class JWTService(IConfiguration config)
{
    private static List<Claim> GenerateClaims(User user)
    {
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new (JwtRegisteredClaimNames.GivenName, user.FirstName),
            new (JwtRegisteredClaimNames.FamilyName, user.LastName),
        };

        var userRoles = Enum.GetValues<Roles>()
            .Where(role => role != Roles.Default && user.Roles.HasFlag(role));
        
        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role.ToString())));
        return claims;
    }

    private SymmetricSecurityKey GenerateSecurityKey()
    {
        var secretKey = config["JWT:Secret"];
        return secretKey == null ? throw new ArgumentNullException(nameof(secretKey)) : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    }

    private static SigningCredentials GenerateSigningCredentials(SymmetricSecurityKey securityKey)
    {
        return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }

    private SecurityTokenDescriptor GenerateTokenDescriptor(User user)
    {
        var claims = GenerateClaims(user);
        var securityKey = GenerateSecurityKey();
        var signingCredentials = GenerateSigningCredentials(securityKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = signingCredentials,
            Issuer = config["JWT:Issuer"],
            Audience = config["JWT:Audience"],
        };
        return tokenDescriptor;
    }

    public string GenerateToken(User user)
    {
        var handler = new JsonWebTokenHandler();
        var tokenDescriptor = GenerateTokenDescriptor(user);
        
        var token = handler.CreateToken(tokenDescriptor);
        
        return token ?? throw new Exception("Generated JWT is null");
    }
}