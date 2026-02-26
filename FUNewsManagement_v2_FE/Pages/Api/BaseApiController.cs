using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;

namespace FUNewsManagement_v2_FE.Pages.Api
{
    /// <summary>
    /// Base controller for all API proxy controllers.
    /// Provides shared helpers to detect when the backend is offline (HttpRequestException
    /// after Polly retries, or BrokenCircuitException when the circuit is open) and
    /// return a standardised 503 response that the global JS interceptor can act on.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Returns true when the exception indicates the backend API is unreachable
        /// (connection refused, timeout after all Polly retries, or circuit open).
        /// </summary>
        protected static bool IsOfflineException(Exception ex) =>
            ex is HttpRequestException ||
            ex is BrokenCircuitException ||
            ex.InnerException is HttpRequestException ||
            ex.InnerException is BrokenCircuitException;

        /// <summary>
        /// Returns HTTP 503 with { offline: true } so the global JS fetch interceptor
        /// in _Layout.cshtml can redirect the user to /Offline immediately.
        /// </summary>
        protected IActionResult OfflineResult() =>
            StatusCode(503, new { offline = true, message = "Máy chủ API đang không khả dụng. Vui lòng thử lại sau." });
    }
}
