using System.Net;
using System.Text.Json;
using PawCast.Api.Models;

namespace PawCast.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "External HTTP request failed.");

            await WriteErrorAsync(
                context,
                HttpStatusCode.BadGateway,
                new ErrorResponse(
                    Code: "external_service_error",
                    Message: "A dependent external service request failed.",
                    TraceId: context.TraceIdentifier));
        }
        catch (TaskCanceledException ex) when (!context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogError(ex, "External request timed out.");

            await WriteErrorAsync(
                context,
                HttpStatusCode.GatewayTimeout,
                new ErrorResponse(
                    Code: "external_service_timeout",
                    Message: "A dependent external service timed out.",
                    TraceId: context.TraceIdentifier));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Application processing failed.");

            await WriteErrorAsync(
                context,
                HttpStatusCode.BadGateway,
                new ErrorResponse(
                    Code: "data_processing_error",
                    Message: ex.Message,
                    TraceId: context.TraceIdentifier));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");

            await WriteErrorAsync(
                context,
                HttpStatusCode.InternalServerError,
                new ErrorResponse(
                    Code: "internal_server_error",
                    Message: "An unexpected error occurred.",
                    TraceId: context.TraceIdentifier));
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        ErrorResponse response)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}