using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CKN.Sdk.Core.Exceptions;

namespace CKN.Sdk.AspNetCore.Middlewares;

/// <summary>
/// A global exception handler that translates domain and validation exceptions into RFC 7807 ProblemDetails format.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An exception occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        if (exception is ValidationException validationException)
        {
            problemDetails.Title = "Validation Error";
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Detail = "One or more validation errors occurred.";

            if (validationException.Errors != null)
            {
                problemDetails.Extensions["errors"] = validationException.Errors;
            }
        }
        else if (exception is CustomException customException)
        {
            problemDetails.Title = "Domain Error";
            problemDetails.Status = customException.StatusCode;
            problemDetails.Detail = customException.Message;

            if (customException.Errors != null)
            {
                problemDetails.Extensions["errors"] = customException.Errors;
            }
        }
        else
        {
            problemDetails.Title = "An unexpected error occurred.";
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Detail = exception.Message;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // Indicates that the exception has been handled
    }
}
