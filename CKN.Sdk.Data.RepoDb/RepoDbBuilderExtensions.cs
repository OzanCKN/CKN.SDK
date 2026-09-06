using System;
using System.Data;
using CKN.Sdk.Core.Data;
using CKN.Sdk.Data.RepoDb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RepoDb;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class RepoDbBuilderExtensions
{
    /// <summary>
    /// Configures the DI container to use RepoDb for PostgreSQL.
    /// </summary>
    public static IServiceCollection AddCknRepoDbPostgres(this IServiceCollection services)
    {
        GlobalConfiguration.Setup().UsePostgreSql();

        services.TryAddScoped(typeof(IRepository<,>), typeof(RepoDbRepository<,>));

        return services;
    }
}
