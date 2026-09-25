namespace CKN.Sdk.Security;

public class CknSecurityOptions
{
    public string JwtSecret { get; set; } = string.Empty;
    public string JwtIssuer { get; set; } = string.Empty;
    public string JwtAudience { get; set; } = string.Empty;
    
    // Default work factor for BCrypt
    public int BCryptWorkFactor { get; set; } = 11;
}
