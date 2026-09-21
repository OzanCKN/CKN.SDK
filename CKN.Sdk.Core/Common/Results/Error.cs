namespace CKN.Sdk.Core.Common.Results;

/// <summary>
/// Represents a discrete error that occurred during application execution.
/// </summary>
public sealed record Error(string Code, string Description)
{
    /// <summary>
    /// Gets the empty error instance.
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>
    /// Gets the null value error instance.
    /// </summary>
    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");
}
