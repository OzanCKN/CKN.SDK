using System.Data;
using CKN.Sdk.Core.Data;
using CKN.Sdk.Data.Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DapperServiceCollectionExtensions
{
    /// <summary>
    /// Configures the DI container to use Dapper for data access.
    /// Note: The caller must provide an IDbConnection registration before calling this.
    /// </summary>
    public static IServiceCollection AddCknDapper(this IServiceCollection services)
    {
        services.TryAddScoped(typeof(IRepository<,>), typeof(DapperRepository<,>));
        services.TryAddScoped<IUnitOfWork, DapperUnitOfWork>();
        
        return services;
    }
}
