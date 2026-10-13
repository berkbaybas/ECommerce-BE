using System.Text.Json;
using Core.Abstraction.Exceptions;
using Core.Abstraction.Responses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Core.Host.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleAsync(context, exception, logger);
        }
    }

    private static Task HandleAsync(HttpContext context, Exception exception, ILogger logger)
    {
        var (statusCode, response) = exception switch
        {
            NotFoundException notFound => (StatusCodes.Status404NotFound, BaseResponse<object>.Fail(notFound.ErrorKey)),
            ValidationException validation => (StatusCodes.Status400BadRequest, BaseResponse<object>.Fail("Validation.Failed", validation.ErrorKeys)),
            BusinessException business => (StatusCodes.Status400BadRequest, BaseResponse<object>.Fail(business.ErrorKey)),
            _ => (StatusCodes.Status500InternalServerError, BaseResponse<object>.Fail("Unexpected.Error")),
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception while processing {Path}", context.Request.Path);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseCoreExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}
