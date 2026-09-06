using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;
using CKN.Sdk.Infrastructure.Identity;

namespace CKN.Sdk.Infrastructure.Security;

public static class AuthenticationConfigurationExtensions
{
    public static IServiceCollection AddCKNAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. JWT Generation Service
        services.AddScoped<JwtTokenGenerator>();

        // 3. Feature Flag Based Authentication configuration
        var featureManager = services.BuildServiceProvider().GetRequiredService<IFeatureManager>();

        var authBuilder = services.AddAuthentication(options =>
        {
            // Default to JWT Bearer
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        // If Local Auth is enabled, add JWT Bearer (Self-Issued)
        if (featureManager.IsEnabledAsync("LocalAuth").GetAwaiter().GetResult())
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "SUPER_SECRET_FALLBACK_KEY_MAKE_SURE_ITS_LONG_ENOUGH_FOR_HMAC256";

            authBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"] ?? "CKN-API",
                    ValidAudience = jwtSettings["Audience"] ?? "CKN-Clients",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
        }

        // If Entra ID is enabled, add Microsoft Identity Web (Entra ID tokens)
        if (featureManager.IsEnabledAsync("EntraIdAuth").GetAwaiter().GetResult())
        {
            // E.g. AddMicrosoftIdentityWebApi
            // Currently handled via additional schemas or we just let it be.
            // Placeholder for real Microsoft Identity Web integration if needed.
        }

        // If Keycloak is enabled, add Keycloak JWT Validation
        if (featureManager.IsEnabledAsync("KeycloakAuth").GetAwaiter().GetResult())
        {
            var keycloakSettings = configuration.GetSection("Keycloak");
            var requireHttps = keycloakSettings.GetValue<bool>("RequireHttpsMetadata", false);

            authBuilder.AddJwtBearer("Keycloak", options =>
            {
                options.Authority = keycloakSettings["Authority"];
                options.Audience = keycloakSettings["Audience"];
                options.RequireHttpsMetadata = requireHttps;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = keycloakSettings["Authority"],
                    ValidateAudience = true,
                    ValidAudience = keycloakSettings["Audience"],
                    ValidateLifetime = true
                };
            });
        }

        // Create a default authorization policy that accepts either Local JWT or Keycloak JWT
        services.AddAuthorization(options =>
        {
            var defaultPolicyBuilder = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder(
                JwtBearerDefaults.AuthenticationScheme,
                "Keycloak");

            defaultPolicyBuilder = defaultPolicyBuilder.RequireAuthenticatedUser();
            options.DefaultPolicy = defaultPolicyBuilder.Build();
        });

        return services;
    }
}
