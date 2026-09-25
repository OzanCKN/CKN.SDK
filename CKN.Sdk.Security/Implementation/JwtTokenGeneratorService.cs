using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using CKN.Sdk.Security.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace CKN.Sdk.Security.Implementation;

public class JwtTokenGeneratorService : ITokenGeneratorService
{
    private readonly CknSecurityOptions _options;

    public JwtTokenGeneratorService(CknSecurityOptions options)
    {
        _options = options;
    }

    public string GenerateToken(CknTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(_options.JwtSecret))
            throw new InvalidOperationException("JWT Secret is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.JwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.UserId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        }.ToList();

        foreach (var role in request.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(request.Expiration),
            Issuer = _options.JwtIssuer,
            Audience = _options.JwtAudience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
