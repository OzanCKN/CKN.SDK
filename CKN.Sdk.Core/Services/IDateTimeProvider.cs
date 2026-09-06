namespace CKN.Sdk.Core.Services;

/// <summary>
/// Abstraction for retrieving the current date and time.
/// Essential for unit testing and mocking time-dependent logic.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current UTC date and time.
    /// </summary>
    DateTime UtcNow { get; }
}

/// <summary>
/// The default implementation of <see cref="IDateTimeProvider"/> using the .NET TimeProvider.
/// </summary>
public class SystemDateTimeProvider : IDateTimeProvider
{
    /// <summary>
    /// Returns the current UTC date and time from the system clock.
    /// </summary>
    public DateTime UtcNow => TimeProvider.System.GetUtcNow().UtcDateTime;
}
