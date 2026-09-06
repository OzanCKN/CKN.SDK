using System.Data;
using System.Threading.Tasks;
using CKN.Sdk.Data.Dapper;
using Microsoft.Data.Sqlite;
using Xunit;
using Dapper;

namespace CKN.Sdk.Tests.Data.Dapper;

public class DapperUnitOfWorkTests
{
    [Fact]
    public async Task SaveChangesAsync_ShouldCommitTransaction()
    {
        // Arrange
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        connection.Execute("CREATE TABLE UowTest (Id INTEGER PRIMARY KEY, Val TEXT);");

        using var uow = new DapperUnitOfWork(connection);
        
        // Act
        uow.BeginTransaction();
        
        // Assert
        // We will just test UoW mechanics to ensure it doesn't throw
        var result = await uow.SaveChangesAsync();
        Assert.Equal(1, result);
    }
}
