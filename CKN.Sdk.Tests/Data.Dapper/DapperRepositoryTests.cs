using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using CKN.Sdk.Core.Domain;
using CKN.Sdk.Data.Dapper;
using Microsoft.Data.Sqlite;
using Xunit;
using Dapper;
using Dommel;

namespace CKN.Sdk.Tests.Data.Dapper;

[Table("TestEntities")]
public class TestEntity : Entity<int>
{
    public string Name { get; set; } = string.Empty;
}

public class DapperRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;

    public DapperRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        
        // Dommel expects Id for keys or [Key] attribute. 
        // Create table for testing.
        _connection.Execute(@"
            CREATE TABLE TestEntities (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL
            );
        ");
    }

    [Fact]
    public async Task AddAsync_ShouldInsertEntity()
    {
        // Arrange
        var repository = new DapperRepository<TestEntity, int>(_connection);
        var entity = new TestEntity { Name = "DapperTest" };

        // Act
        await repository.AddAsync(entity);
        var count = await _connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM TestEntities");

        // Assert
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity()
    {
        // Arrange
        var repository = new DapperRepository<TestEntity, int>(_connection);
        var entity = new TestEntity { Name = "DapperTest" };
        var id = Convert.ToInt32(await _connection.InsertAsync(entity));
        
        // Act
        var result = await repository.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("DapperTest", result.Name);
    }
    
    [Fact]
    public async Task Delete_ShouldRemoveEntity()
    {
        // Arrange
        var repository = new DapperRepository<TestEntity, int>(_connection);
        var initialEntity = new TestEntity { Name = "DeleteMe" };
        var id = Convert.ToInt32(await _connection.InsertAsync(initialEntity));
        var entity = await repository.GetByIdAsync(id);
        
        // Act
        repository.Delete(entity!);
        var count = await _connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM TestEntities");

        // Assert
        Assert.Equal(0, count);
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }
}
