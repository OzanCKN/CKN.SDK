using System;
using CKN.Sdk.Security.Abstractions;
using CKN.Sdk.Security.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace CKN.Sdk.Security;

public static class CknSecurityServiceCollectionExtensions
{
    public static IServiceCollection AddCknSecurity(this IServiceCollection services, Action<CknSecurityOptions> configureOptions)
    {
        var options = new CknSecurityOptions();
        configureOptions(options);

        services.AddSingleton(options);
        
        services.AddSingleton<IPasswordHasherService, BCryptPasswordHasherService>();
        services.AddSingleton<ITokenGeneratorService, JwtTokenGeneratorService>();

        return services;
    }
}
