using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Common.Results;
using CKN.Sdk.Network.Abstractions;
using CKN.Sdk.Network.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CKN.Sdk.Network.Http.Services;

/// <summary>
/// Abstract base class for strongly-typed HTTP clients that use <see cref="ICknHttpClient"/>.
/// Derive from this class and inject it via <c>AddCknHttpClient&lt;TClient&gt;</c>.
/// </summary>
/// <example>
/// <code>
/// public class YahooFinanceProvider : CknHttpClientBase
/// {
///     public YahooFinanceProvider(
///         HttpClient httpClient,
///         ILoggerFactory loggerFactory,
///         IOptionsMonitor&lt;CknHttpClientOptions&gt; options)
///         : base(httpClient, loggerFactory, options) { }
///
///     public Task&lt;Result&lt;ChartResponse&gt;&gt; GetChartAsync(string symbol, CancellationToken ct = default)
///         =&gt; GetJsonAsync(
///             $"v8/finance/chart/{symbol}",
///             YahooJsonContext.Default.ChartResponse,
///             ct);
/// }
/// </code>
/// </example>
public abstract class CknHttpClientBase : ICknHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly string _clientName;
    private readonly int _maxDegreeOfParallelism;

    /// <summary>
    /// Initialises the base HTTP client.
    /// </summary>
    /// <param name="httpClient">Injected <see cref="HttpClient"/> configured by DI.</param>
    /// <param name="loggerFactory">Factory used to create a logger named after this client type.</param>
    /// <param name="options">Named options monitor; the entry named after this type is used.</param>
    protected CknHttpClientBase(
        HttpClient httpClient,
        ILoggerFactory loggerFactory,
        IOptionsMonitor<CknHttpClientOptions> options)
    {
        _clientName = GetType().Name;
        _httpClient = httpClient;
        _logger = loggerFactory.CreateLogger(_clientName);

        var opts = options.Get(_clientName);
        _maxDegreeOfParallelism = opts.MaxDegreeOfParallelism > 0
            ? opts.MaxDegreeOfParallelism
            : 4;
    }

    /// <inheritdoc />
    public Task<Result<T>> GetJsonAsync<T>(string relativeUrl, CancellationToken ct = default)
        => GetJsonAsync<T>(relativeUrl, jsonTypeInfo: null, ct);

    /// <inheritdoc />
    public async Task<Result<T>> GetJsonAsync<T>(
        string relativeUrl,
        JsonTypeInfo<T>? jsonTypeInfo,
        CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            using var response = await _httpClient
                .GetAsync(relativeUrl, HttpCompletionOption.ResponseHeadersRead, ct)
                .ConfigureAwait(false);

            sw.Stop();

            if (!response.IsSuccessStatusCode)
            {
                var code = $"HTTP_{(int)response.StatusCode}";
                var desc = response.ReasonPhrase ?? response.StatusCode.ToString();
                return Result.Failure<T>(new Error(code, desc));
            }

            await using var stream = await response.Content
                .ReadAsStreamAsync(ct)
                .ConfigureAwait(false);

            T? value = jsonTypeInfo is not null
                ? await JsonSerializer.DeserializeAsync(stream, jsonTypeInfo, ct).ConfigureAwait(false)
                : await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: ct).ConfigureAwait(false);

            return value is null
                ? Result.Failure<T>(Error.NullValue)
                : Result.Success(value);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex,
                "[{ClientName}] GET {Url} failed after {ElapsedMs}ms",
                _clientName, relativeUrl, sw.ElapsedMilliseconds);
            return Result.Failure<T>(new Error("HTTP_NETWORK", ex.Message));
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Result<T>>> BatchAsync<T>(
        IEnumerable<string> relativeUrls,
        JsonTypeInfo<T> jsonTypeInfo,
        CancellationToken ct = default)
    {
        var urls = new List<string>(relativeUrls);
        var results = new Result<T>[urls.Count];

        using var semaphore = new SemaphoreSlim(_maxDegreeOfParallelism, _maxDegreeOfParallelism);
        var tasks = new Task[urls.Count];

        for (var i = 0; i < urls.Count; i++)
        {
            var index = i;
            var url = urls[i];

            tasks[i] = Task.Run(async () =>
            {
                await semaphore.WaitAsync(ct).ConfigureAwait(false);
                try
                {
                    results[index] = await GetJsonAsync(url, jsonTypeInfo, ct).ConfigureAwait(false);
                }
                finally
                {
                    semaphore.Release();
                }
            }, ct);
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
        return results;
    }
}
