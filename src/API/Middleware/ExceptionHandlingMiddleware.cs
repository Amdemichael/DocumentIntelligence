using System.Text.Json;
using FluentValidation;
using Application.Common.Exceptions;
using Domain.Exceptions;

namespace API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        var response = context.Response;
        response.ContentType = "application/json";

        int statusCode;
        object errorResponse;

        // Use if/else instead of switch to avoid unreachable code issues
        if (exception is ValidationException validationException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            errorResponse = new
            {
                error = new
                {
                    message = "Validation failed",
                    type = "ValidationException",
                    errors = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                }
            };
        }
        else if (exception is NotFoundException)
        {
            statusCode = StatusCodes.Status404NotFound;
            errorResponse = new
            {
                error = new
                {
                    message = exception.Message,
                    type = "NotFoundException"
                }
            };
        }
        else if (exception is DomainException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            errorResponse = new
            {
                error = new
                {
                    message = exception.Message,
                    type = "DomainException"
                }
            };
        }
        else if (exception is InvalidDocumentStateException)
        {
            statusCode = StatusCodes.Status409Conflict;
            errorResponse = new
            {
                error = new
                {
                    message = exception.Message,
                    type = "InvalidDocumentStateException"
                }
            };
        }
        else
        {
            statusCode = StatusCodes.Status500InternalServerError;
            errorResponse = new
            {
                error = new
                {
                    message = "An internal error occurred. Please try again later.",
                    type = "InternalServerError"
                }
            };
        }

        response.StatusCode = statusCode;

        var jsonResult = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResult);
    }
}