using API.Models;
using Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Filters;

/// <summary>
/// Global exception filter to handle all exceptions and return standardized error responses
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        ApiErrorResponse errorResponse;
        int statusCode;

        switch (exception)
        {
            case ValidationException validationEx:
                statusCode = StatusCodes.Status400BadRequest;
                errorResponse = ApiErrorResponse.Create(
                    "فشل التحقق من الصحة",
                    validationEx.Errors
                );
                break;

            case UnauthorizedException unauthorizedEx:
                statusCode = StatusCodes.Status401Unauthorized;
                errorResponse = ApiErrorResponse.Create(unauthorizedEx.Message);
                break;

            case NotFoundException notFoundEx:
                statusCode = StatusCodes.Status404NotFound;
                errorResponse = ApiErrorResponse.Create(notFoundEx.Message);
                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                errorResponse = ApiErrorResponse.Create(
                    "حدث خطأ في الخادم. يرجى المحاولة مرة أخرى لاحقاً"
                );
                break;
        }

        context.Result = new ObjectResult(errorResponse)
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }
}
