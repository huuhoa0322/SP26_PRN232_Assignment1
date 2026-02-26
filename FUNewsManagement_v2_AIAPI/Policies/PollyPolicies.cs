using Polly;
using Polly.Extensions.Http;
using System.Net;

namespace FUNewsManagement_v2_AIAPI.Policies
{
    /// <summary>
    /// Polly resilience policies for outbound calls to the Google Gemini API.
    /// Different from the FE policies because:
    ///   • Handles 429 Too Many Requests (Google rate-limit).
    ///   • Adds random jitter to avoid thundering-herd on quota resets.
    ///   • Circuit-breaker break duration is 60 s (external APIs recover slower).
    /// </summary>
    public static class PollyPolicies
    {
        /// <summary>
        /// Retry policy for the Google Gemini API.
        /// 3 attempts, exponential back-off 2 s → 4 s → 8 s plus up to 1 s of jitter.
        /// Triggers on: network errors, 5xx and 429 Too Many Requests.
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> GetAiApiRetryPolicy(ILogger logger)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, attempt))
                        + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 1000)),
                    onRetry: (DelegateResult<HttpResponseMessage> outcome, TimeSpan delay, int attempt, Context ctx) =>
                        logger.LogWarning(
                            "[Polly] Gemini API Retry {Attempt}/3 after {Delay:F1}s — {Reason}",
                            attempt,
                            delay.TotalSeconds,
                            outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()));
        }

        /// <summary>
        /// Circuit-breaker for the Gemini API.
        /// Opens after 5 consecutive failures, stays open for 60 s
        /// (longer than internal services — external APIs need more recovery time).
        /// Must be a singleton — create once and reuse across requests.
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ILogger logger)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(60),
                    onBreak: (DelegateResult<HttpResponseMessage> outcome, TimeSpan duration) =>
                        logger.LogError(
                            "[Polly] Gemini API Circuit OPEN for {Duration}s — {Reason}",
                            duration.TotalSeconds,
                            outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()),
                    onReset: () => logger.LogInformation("[Polly] Gemini API Circuit CLOSED — requests resuming normally"),
                    onHalfOpen: () => logger.LogInformation("[Polly] Gemini API Circuit HALF-OPEN — testing with next request"));
        }
    }
}
