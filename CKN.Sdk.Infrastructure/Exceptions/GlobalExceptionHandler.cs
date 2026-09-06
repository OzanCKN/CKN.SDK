using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CKN.Sdk.Infrastructure.Exceptions;

/// <summary>
/// Global exception handler middleware using .NET 8 IExceptionHandler.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path,
            Title = "An error occurred while processing your request.",
        };

        if (exception is CustomException customException)
        {
            httpContext.Response.StatusCode = customException.StatusCode;
            problemDetails.Status = customException.StatusCode;
            problemDetails.Detail = customException.Message;

            if (customException.Errors != null)
            {
                problemDetails.Extensions["errors"] = customException.Errors;
            }
        }
        else
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Detail = "Internal Server Error";
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
