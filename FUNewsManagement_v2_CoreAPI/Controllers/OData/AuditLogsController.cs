using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.AuditLog;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagement_v2_CoreAPI.Controllers.OData
{
    [Authorize(Roles = "Admin")]
    public class AuditLogsController : ODataController
    {
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<AuditLogsController> _logger;

        public AuditLogsController(IAuditLogService auditLogService, ILogger<AuditLogsController> logger)
        {
            _auditLogService = auditLogService;
            _logger = logger;
        }

        [EnableQuery(MaxTop = 100)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var logs = await _auditLogService.GetAllAsync();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OData GetAuditLogs");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
