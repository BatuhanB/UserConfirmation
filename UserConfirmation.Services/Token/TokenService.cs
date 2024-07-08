using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserConfirmation.Data.Models;
using UserConfirmation.Shared.Models;

namespace UserConfirmation.Services.Token;
public class TokenService(
    IOptions<JwtSettings> options, 
    UserManager<ApplicationUser> userManager) : ITokenService
{
    private readonly JwtSettings _jwtSettings = options.Value;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<string> GenerateToken(ApplicationUser user)
    {
        ClaimsIdentity claims = await GenerateClaims(user);

        var handler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
            SigningCredentials= credentials,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    private async Task<ClaimsIdentity> GenerateClaims(ApplicationUser user)
    {
        var claims = new ClaimsIdentity();
        claims.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
        claims.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.AddClaim(new Claim(ClaimTypes.Name, user.UserName!));
        claims.AddClaim(new Claim(ClaimTypes.Email, user.Email!));


        var userRoles = await _userManager.GetRolesAsync(user);

        foreach (var role in userRoles)
        {
            claims.AddClaim(new Claim(ClaimTypes.Role, role.ToString()));
        }

        return claims;
    }
}
