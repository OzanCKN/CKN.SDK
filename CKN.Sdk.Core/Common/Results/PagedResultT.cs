using System.Collections.Generic;

namespace CKN.Sdk.Core.Common.Results;

/// <summary>
/// Represents a paged result of some operation, with status information, a collection of values, and pagination details.
/// </summary>
/// <typeparam name="TValue">The result value type.</typeparam>
public class PagedResult<TValue> : Result<IReadOnlyList<TValue>>
{
    protected internal PagedResult(IReadOnlyList<TValue>? value, bool isSuccess, Error error, int pageNumber, int pageSize, long totalRecords)
        : base(value, isSuccess, error)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;
    }

    /// <summary>
    /// Gets the current page number.
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the total number of records across all pages.
    /// </summary>
    public long TotalRecords { get; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => PageSize == 0 ? 0 : (int)System.Math.Ceiling(TotalRecords / (double)PageSize);

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Returns a success <see cref="PagedResult{TValue}"/> with the specified values and pagination details.
    /// </summary>
    public static PagedResult<TValue> Success(IReadOnlyList<TValue> value, int pageNumber, int pageSize, long totalRecords) => 
        new(value, true, Error.None, pageNumber, pageSize, totalRecords);

    /// <summary>
    /// Creates a failure <see cref="PagedResult{TValue}"/> with the specified error.
    /// </summary>
    public static new PagedResult<TValue> Failure(Error error) => 
        new(default, false, error, 0, 0, 0);
}
