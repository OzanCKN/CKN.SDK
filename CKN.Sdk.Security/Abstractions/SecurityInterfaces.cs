using System;
using System.Collections.Generic;

namespace CKN.Sdk.Security.Abstractions;

public class CknTokenRequest
{
    public string UserId { get; set; } = string.Empty;
    public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();
    public TimeSpan Expiration { get; set; } = TimeSpan.FromHours(1);
}

public interface ITokenGeneratorService
{
    string GenerateToken(CknTokenRequest request);
}

public interface IPasswordHasherService
{
    string HashPassword(string plainText);
    bool VerifyPassword(string plainText, string hash);
}
