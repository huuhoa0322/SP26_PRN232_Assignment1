using Polly;
using Polly.Extensions.Http;

namespace FUNewsManagement_v2_FE.Policies
{
    /// <summary>
    /// Centralized Polly resilience policies for HttpClient registrations.
    /// </summary>
    public static class PollyPolicies
    {
        /// <summary>
        /// Retry policy: 3 attempts with exponential back-off (2 s → 4 s → 8 s).
        /// Triggered on transient HTTP errors (5xx, network timeout, connection refused).
        /// A warning is logged on every retry attempt so developers can observe the behavior.
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger) =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (outcome, timespan, attempt, _) =>
                        logger.LogWarning(
                            "[Polly] Retry {Attempt}/3 after {Delay:F1}s — {Reason}",
                            attempt,
                            timespan.TotalSeconds,
                            outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()));

        /// <summary>
        /// Circuit-breaker policy: opens after 5 consecutive failures, stays open for 30 s.
        /// While open, all requests fail immediately (fail-fast) instead of waiting for timeout.
        /// Must be a singleton — create once per named HttpClient and reuse across requests.
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ILogger logger) =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (outcome, duration) =>
                        logger.LogError(
                            "[Polly] Circuit OPEN for {Duration}s — {Reason}",
                            duration.TotalSeconds,
                            outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()),
                    onReset: () => logger.LogInformation("[Polly] Circuit CLOSED — requests resuming normally"),
                    onHalfOpen: () => logger.LogInformation("[Polly] Circuit HALF-OPEN — testing with next request"));
    }
}
