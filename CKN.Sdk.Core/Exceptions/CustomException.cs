using System;
using System.Collections.Generic;

namespace CKN.Sdk.Core.Exceptions;

/// <summary>
/// Base class for all domain custom exceptions.
/// </summary>
public abstract class CustomException : Exception
{
    /// <summary>
    /// Gets the HTTP status code associated with this exception.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets the dictionary of validation errors, if any.
    /// </summary>
    public IReadOnlyDictionary<string, string[]>? Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errors">Optional dictionary of validation errors.</param>
    protected CustomException(string message, int statusCode, IReadOnlyDictionary<string, string[]>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}
