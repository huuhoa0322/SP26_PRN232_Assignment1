using FUNewsManagement_v2_FE.Services;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagement_v2_FE.Pages.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly CoreApiService _apiService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(CoreApiService apiService, ILogger<ProfileController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // Check if session has token
                var token = HttpContext.Session.GetString("JwtToken");
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("No JWT token found in session");
                    return Unauthorized("No authentication token found");
                }

                var result = await _apiService.GetProfileAsync();
                if (result == null)
                    return StatusCode(500, "Failed to fetch profile");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProfile");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
