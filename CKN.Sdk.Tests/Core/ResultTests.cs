using CKN.Sdk.Core.Common.Results;
using FluentAssertions;
using Xunit;

namespace CKN.Sdk.Tests.Core;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ShouldCreateFailureResult()
    {
        var error = new Error("Test.Error", "Test error description");
        var result = Result.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Success_WithGenericValue_ShouldCreateSuccessfulResult()
    {
        var result = Result<string>.Success("Test Value");

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
        result.Value.Should().Be("Test Value");
    }

    [Fact]
    public void Failure_WithGenericValue_ShouldCreateFailureResult()
    {
        var error = new Error("Test.Error", "Test error description");
        var result = Result.Failure<string>(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        
        var act = () => result.Value;
        act.Should().Throw<InvalidOperationException>();
    }
    
    [Fact]
    public void PagedResult_Success_ShouldCreateSuccessfulPagedResult()
    {
        var items = new List<string> { "item1", "item2" };
        var result = PagedResult<string>.Success(items, 1, 10, 2);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(items);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalRecords.Should().Be(2);
        result.TotalPages.Should().Be(1);
        result.HasNextPage.Should().BeFalse();
        result.HasPreviousPage.Should().BeFalse();
    }
}
