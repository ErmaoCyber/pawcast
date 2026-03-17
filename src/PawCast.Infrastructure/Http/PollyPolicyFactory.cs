using System.Net;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace PawCast.Infrastructure.Http;

public static class PollyPolicyFactory
{
    public static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(ILogger logger)
    {
        return Polly.Extensions.Http.HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TaskCanceledException>()
            .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (outcome, delay, retryAttempt, context) =>
                {
                    if (outcome.Exception is not null)
                    {
                        logger.LogWarning(
                            outcome.Exception,
                            "Retrying external HTTP request due to exception. Attempt={RetryAttempt}, DelayMs={DelayMs}",
                            retryAttempt,
                            delay.TotalMilliseconds);
                    }
                    else
                    {
                        logger.LogWarning(
                            "Retrying external HTTP request due to status code {StatusCode}. Attempt={RetryAttempt}, DelayMs={DelayMs}",
                            (int)outcome.Result.StatusCode,
                            retryAttempt,
                            delay.TotalMilliseconds);
                    }
                });
    }
}