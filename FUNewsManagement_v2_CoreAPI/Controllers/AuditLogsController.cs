using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.AuditLog;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagement_v2_CoreAPI.Controllers
{
    /// <summary>
    /// Controller cho Audit Log Management (Admin only)
    /// </summary>
    [ApiController]
    [Authorize(Roles = "Admin")] // ⭐ ADMIN ONLY
    public class AuditLogsController : ODataController
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// GET /odata/AuditLogs - List audit logs với OData queries
        /// Supports: $select, $filter, $orderby, $top, $skip, $count
        /// Example filters:
        /// - $filter=UserId eq 1
        /// - $filter=Entity eq 'Account'
        /// - $filter=Timestamp ge 2024-01-01T00:00:00Z
        /// </summary>
        [HttpGet("odata/AuditLogs")]
        [EnableQuery(MaxTop = 100)]
        public async Task<IActionResult> Get()
        {
            var logs = await _auditLogService.GetAllAsync();
            return Ok(logs);
        }

        /// <summary>
        /// GET /odata/AuditLogs({id}) - Get audit log detail
        /// </summary>
        [HttpGet("odata/AuditLogs({id})")]
        [EnableQuery]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var log = await _auditLogService.GetByIdAsync(id);
            if (log == null)
            {
                return NotFound(new { message = "Audit log không tồn tại" });
            }

            return Ok(log);
        }

        /// <summary>
        /// GET /api/auditlogs/filter - Filter audit logs
        /// Query params: userId, entity, fromDate, toDate
        /// </summary>
        [HttpGet("api/auditlogs/filter")]
        public async Task<IActionResult> Filter(
            [FromQuery] short? userId,
            [FromQuery] string? entity,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var logs = await _auditLogService.FilterAsync(userId, entity, fromDate, toDate);
            return Ok(logs);
        }
    }
}
