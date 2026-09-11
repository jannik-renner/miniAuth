using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniAuth.Application.Common.Exceptions;
using FluentValidation;

namespace MiniAuth.Api.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Unhandled exception occurred.");

            if (exception is ValidationException validationException)
            {
                var validationProblemDetails = new ValidationProblemDetails(
                    validationException.Errors
                        .GroupBy(x => x.PropertyName)
                        .ToDictionary(
                            group => group.Key,
                            group => group
                                .Select(x => x.ErrorMessage)
                                .ToArray()))
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation failed."
                };

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                await httpContext.Response.WriteAsJsonAsync(validationProblemDetails, cancellationToken);

                return true;
            }

            var statusCode = exception switch
            {
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ConflictException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = statusCode switch
                {
                    StatusCodes.Status401Unauthorized => "Unauthorized",
                    StatusCodes.Status409Conflict => "Conflict",
                    _ => "An unexpected error occurred."
                }
            };

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
