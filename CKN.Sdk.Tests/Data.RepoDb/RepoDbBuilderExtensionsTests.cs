using System;
using CKN.Sdk.Core.Data;
using CKN.Sdk.Core.Domain;
using CKN.Sdk.Data.RepoDb;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CKN.Sdk.Tests.Data.RepoDb;

public class TestRepoDbEntity : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
}

public class RepoDbBuilderExtensionsTests
{
    [Fact]
    public void AddCknRepoDbPostgres_ShouldRegisterRepository()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Mock a DB Connection for RepoDb resolving
        services.AddScoped<System.Data.IDbConnection>(sp => new Npgsql.NpgsqlConnection());
        
        // Act
        services.AddCknRepoDbPostgres();
        
        var serviceProvider = services.BuildServiceProvider();
        var repository = serviceProvider.GetService<IRepository<TestRepoDbEntity, Guid>>();

        // Assert
        Assert.NotNull(repository);
        Assert.IsType<RepoDbRepository<TestRepoDbEntity, Guid>>(repository);
    }
}
