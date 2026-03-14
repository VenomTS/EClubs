using System.Security.Claims;
using System.Text;
using E_Clubs.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace E_Clubs.Services;

public class JWTService(IConfiguration config)
{
    private IEnumerable<Claim> GenerateClaims(User user, IEnumerable<Role> roles)
    {
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));
        return claims;
    }

    private SymmetricSecurityKey GenerateSecurityKey()
    {
        var secretKey = config["JWT:Secret"];
        return secretKey == null ? throw new ArgumentNullException(nameof(secretKey)) : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    }

    private SigningCredentials GenerateSigningCredentials(SymmetricSecurityKey securityKey)
    {
        return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }

    private SecurityTokenDescriptor GenerateTokenDescriptor(User user, IEnumerable<Role> roles)
    {
        var claims = GenerateClaims(user, roles);
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

    public string GenerateToken(User user, IEnumerable<Role> roles)
    {
        var handler = new JsonWebTokenHandler();
        var tokenDescriptor = GenerateTokenDescriptor(user, roles);
        
        var token = handler.CreateToken(tokenDescriptor);
        
        return token ?? throw new Exception("Generated JWT is null");
    }
}