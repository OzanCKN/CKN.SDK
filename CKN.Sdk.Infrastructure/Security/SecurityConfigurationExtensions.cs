using Microsoft.AspNetCore.Builder;

namespace CKN.Sdk.Infrastructure.Security;

/// <summary>
/// Extension methods for registering security components.
/// </summary>
public static class SecurityConfigurationExtensions
{
    /// <summary>
    /// Adds the hardware-locked ECDSA license validation middleware to the pipeline.
    /// This will block any requests if the license is invalid, expired, or the HWID mismatches.
    /// </summary>
    /// <param name="app">The application builder.</param>
    public static IApplicationBuilder UseCKNLicenseValidation(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LicenseValidationMiddleware>();
    }
}
