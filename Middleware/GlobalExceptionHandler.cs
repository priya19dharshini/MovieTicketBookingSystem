using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingSystem.Exceptions;

namespace MovieTicketBookingSystem.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            BadRequestException =>
                (StatusCodes.Status400BadRequest,
                 "Bad Request",
                 exception.Message),

            ArgumentException =>
                (StatusCodes.Status400BadRequest,
                 "Bad Request",
                 exception.Message),

            System.ComponentModel.DataAnnotations.ValidationException =>
                (StatusCodes.Status400BadRequest,
                 "Validation Failed",
                 exception.Message),

            NotFoundException =>
                (StatusCodes.Status404NotFound,
                 "Resource Not Found",
                 exception.Message),

            KeyNotFoundException =>
                (StatusCodes.Status404NotFound,
                 "Resource Not Found",
                 exception.Message),

            ConflictException =>
                (StatusCodes.Status409Conflict,
                 "Conflict",
                 exception.Message),

            InvalidOperationException =>
                (StatusCodes.Status409Conflict,
                 "Conflict",
                 exception.Message),

            _ =>
                (StatusCodes.Status500InternalServerError,
                 "Internal Server Error",
                 "An unexpected error occurred. Please try again later.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "An unexpected error occurred. TraceId: {TraceId}",
                httpContext.TraceIdentifier);
        }
        else
        {
            _logger.LogWarning(
                "Request failed with status {StatusCode}. " +
                "Message: {Message}. TraceId: {TraceId}",
                statusCode,
                exception.Message,
                httpContext.TraceIdentifier);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] =
            httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}