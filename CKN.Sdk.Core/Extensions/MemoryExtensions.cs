using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace CKN.Sdk.Core.Extensions;

/// <summary>
/// Provides high-performance extension methods utilizing Span and Memory types
/// to reduce memory allocations in the CKN SDK.
/// </summary>
public static class MemoryExtensions
{
    /// <summary>
    /// Deserializes JSON directly from a ReadOnlySpan of bytes for high performance.
    /// </summary>
    public static T? DeserializeFromSpan<T>(this ReadOnlySpan<byte> utf8Json, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(utf8Json, options);
    }

    /// <summary>
    /// Splits a string efficiently using ReadOnlySpan without allocating new strings.
    /// </summary>
    public static void SplitSpan(this ReadOnlySpan<char> span, char separator, out ReadOnlySpan<char> first, out ReadOnlySpan<char> second)
    {
        int index = span.IndexOf(separator);
        if (index == -1)
        {
            first = span;
            second = ReadOnlySpan<char>.Empty;
        }
        else
        {
            first = span.Slice(0, index);
            second = span.Slice(index + 1);
        }
    }

    /// <summary>
    /// Checks if a ReadOnlySpan of characters contains a specific character.
    /// </summary>
    public static bool ContainsFast(this ReadOnlySpan<char> span, char value)
    {
        return span.IndexOf(value) >= 0;
    }
}
