using FUNewsManagement_v2_FE.Services;
using Microsoft.AspNetCore.Mvc;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Dashboard;

namespace FUNewsManagement_v2_FE.Pages.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly CoreApiService _apiService;

        public DashboardController(CoreApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var result = await _apiService.GetDashboardStatsAsync();
            if (result == null)
                return StatusCode(500);

            return Ok(result);
        }
    }
}
 