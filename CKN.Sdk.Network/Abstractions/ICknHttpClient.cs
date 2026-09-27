using System.Collections.Generic;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Common.Results;

namespace CKN.Sdk.Network.Abstractions;

/// <summary>
/// Represents a strongly typed, resilient HTTP client that returns
/// <see cref="Result{T}"/> instead of throwing exceptions.
/// </summary>
public interface ICknHttpClient
{
    /// <summary>
    /// Sends a GET request to the specified relative URL and deserializes the JSON response.
    /// Uses reflection-based deserialization. Prefer the <see cref="JsonTypeInfo{T}"/> overload for Native AOT.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response body into.</typeparam>
    /// <param name="relativeUrl">The path and query relative to the configured base address.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// <see cref="Result{T}"/> with the deserialized value on success,
    /// or a failure result containing an <see cref="Error"/> with code <c>HTTP_{statusCode}</c> or <c>HTTP_NETWORK</c>.
    /// </returns>
    Task<Result<T>> GetJsonAsync<T>(string relativeUrl, CancellationToken ct = default);

    /// <summary>
    /// Sends a GET request and deserializes the JSON response using a source-generated
    /// <see cref="JsonTypeInfo{T}"/>. Recommended for Native AOT compatibility.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response body into.</typeparam>
    /// <param name="relativeUrl">The path and query relative to the configured base address.</param>
    /// <param name="jsonTypeInfo">Source-generated type metadata for zero-reflection deserialization.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A <see cref="Result{T}"/> indicating success or failure.</returns>
    Task<Result<T>> GetJsonAsync<T>(string relativeUrl, JsonTypeInfo<T> jsonTypeInfo, CancellationToken ct = default);

    /// <summary>
    /// Sends GET requests for each URL in <paramref name="relativeUrls"/> with bounded parallelism
    /// and client-side rate limiting. Designed for bulk symbol fetches.
    /// </summary>
    /// <typeparam name="T">The type to deserialize each response into.</typeparam>
    /// <param name="relativeUrls">The relative URLs to fetch, one per item.</param>
    /// <param name="jsonTypeInfo">Source-generated type metadata.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// An ordered list of <see cref="Result{T}"/> values, one per input URL, in the same order.
    /// </returns>
    Task<IReadOnlyList<Result<T>>> BatchAsync<T>(
        IEnumerable<string> relativeUrls,
        JsonTypeInfo<T> jsonTypeInfo,
        CancellationToken ct = default);
}
