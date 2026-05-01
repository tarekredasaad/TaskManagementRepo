using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Dtos;
using Domain.Constants;
using Microsoft.IdentityModel.Tokens;

namespace MyProject.Infrastructure.Helper;

public static class JwtHelper
{
    public static string GenerateToken(UserDto user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(JwtSetting.Key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(CustomClaimTypes.Id, user.Id.ToString()),
                new Claim(CustomClaimTypes.RoleId, user.Role),
                new Claim(CustomClaimTypes.Role, user.Role),
                new Claim(CustomClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(JwtSetting.DurationInMinutes),
            Issuer = JwtSetting.Issuer,
            Audience = JwtSetting.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
