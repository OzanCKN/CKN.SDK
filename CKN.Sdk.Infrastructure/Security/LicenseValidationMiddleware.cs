using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace CKN.Sdk.Infrastructure.Security;

/// <summary>
/// Validates the presence and integrity of a hardware-locked ECDSA license.
/// </summary>
public class LicenseValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LicenseValidationMiddleware> _logger;
    private static bool _isLicenseValid = false;
    private static bool _hasCheckedLicense = false;

    public LicenseValidationMiddleware(RequestDelegate next, ILogger<LicenseValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip validation in Testing environment to allow Integration Tests to run
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Testing")
        {
            await _next(context);
            return;
        }

        if (!_hasCheckedLicense)
        {
            ValidateLicense();
            _hasCheckedLicense = true;
        }

        if (!_isLicenseValid)
        {
            context.Response.StatusCode = 403; // Forbidden
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"Valid license not found or hardware ID mismatch.\"}");
            return;
        }

        await _next(context);
    }

    private void ValidateLicense()
    {
        try
        {
            var (licensePath, publicKeyPath) = ResolveLicensePaths();

            if (string.IsNullOrEmpty(licensePath) || string.IsNullOrEmpty(publicKeyPath))
            {
                _logger.LogError("License files (license.key / public.pem) could not be found! Please run 'GENERATE_LICENSE.bat' or use 'dotnet run --project src/Tools/CKN.Tools.Keygen' to generate a valid license.");
                _isLicenseValid = false;
                return;
            }

            _logger.LogInformation("Loading license files from: {LicensePath} and {PublicKeyPath}", licensePath, publicKeyPath);

            var tokenString = File.ReadAllText(licensePath).Trim();
            var publicKeyPem = File.ReadAllText(publicKeyPath).Trim();

            using var ecdsa = ECDsa.Create();
            ecdsa.ImportFromPem(publicKeyPem);

            var key = new ECDsaSecurityKey(ecdsa);

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "CKN-Keygen",
                ValidateAudience = true,
                ValidAudience = "CKN-Enterprise-Node",
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(tokenString, validationParameters, out var validatedToken);

            // Check Hardware ID
            var hwidClaim = principal.Claims.FirstOrDefault(c => c.Type == "hwid")?.Value;
            var currentHwid = HardwareIdGenerator.GetHardwareId();

            // Support an override or wildcard for dev environments if needed, but strict mode is default
            if (hwidClaim != currentHwid && hwidClaim != "ANY")
            {
                _logger.LogError("License hardware ID mismatch! Expected: {Expected}, Actual: {Actual}. Re-run GENERATE_LICENSE.bat on this machine.", hwidClaim, currentHwid);
                _isLicenseValid = false;
                return;
            }

            _logger.LogInformation("License successfully validated for Tenant: {TenantId}, Features: {Features}",
                principal.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value,
                principal.Claims.FirstOrDefault(c => c.Type == "features")?.Value);

            _isLicenseValid = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate license. It might be expired, corrupted, or forged.");
            _isLicenseValid = false;
        }
    }

    private static (string? licensePath, string? publicKeyPath) ResolveLicensePaths()
    {
        var searchDirectories = new[]
        {
            Directory.GetCurrentDirectory(),
            AppContext.BaseDirectory,
            Path.Combine(Directory.GetCurrentDirectory(), "src", "Products", "CKN.CorporateMemory", "CKN.CorporateMemory.Api"),
            Path.Combine(AppContext.BaseDirectory, "..", "..", ".."),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Products", "CKN.CorporateMemory", "CKN.CorporateMemory.Api")
        };

        foreach (var dir in searchDirectories)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    continue;
                }

                var lic = Path.Combine(dir, "license.key");
                var pub = Path.Combine(dir, "public.pem");

                if (File.Exists(lic) && File.Exists(pub))
                {
                    return (Path.GetFullPath(lic), Path.GetFullPath(pub));
                }
            }
            catch
            {
                // Ignore search error and continue to next directory
            }
        }

        return (null, null);
    }
}
