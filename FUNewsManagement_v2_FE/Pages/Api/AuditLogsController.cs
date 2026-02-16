using FUNewsManagement_v2_FE.Services;
using Microsoft.AspNetCore.Mvc;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.AuditLog;

namespace FUNewsManagement_v2_FE.Pages.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {
        private readonly CoreApiService _apiService;

        public AuditLogsController(CoreApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuditLogs([FromQuery] string? filter, [FromQuery] int? top, [FromQuery] int? skip, [FromQuery] string? orderby)
        {
            var result = await _apiService.GetAuditLogsAsync(filter, top);
            if (result == null)
                return StatusCode(500);

            return Ok(new
            {
                value = result.Value,
                count = result.Count
            });
        }
    }
} 
