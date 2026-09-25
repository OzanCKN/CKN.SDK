using CKN.Sdk.Security.Abstractions;

namespace CKN.Sdk.Security.Implementation;

public class BCryptPasswordHasherService : IPasswordHasherService
{
    private readonly CknSecurityOptions _options;

    public BCryptPasswordHasherService(CknSecurityOptions options)
    {
        _options = options;
    }

    public string HashPassword(string plainText)
    {
        return BCrypt.Net.BCrypt.HashPassword(plainText, _options.BCryptWorkFactor);
    }

    public bool VerifyPassword(string plainText, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(plainText, hash);
    }
}
