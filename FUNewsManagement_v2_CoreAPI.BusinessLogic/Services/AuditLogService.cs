using AutoMapper;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.AuditLog;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using FUNewsManagement_v2_CoreAPI.DataAccess.Models;
using FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Services
{
    /// <summary>
    /// Triển khai Audit Log service
    /// </summary>
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogService(
            IAuditLogRepository auditLogRepo,
            IAccountRepository accountRepo,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _auditLogRepo = auditLogRepo;
            _accountRepo = accountRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<AuditLogDto>> GetAllAsync()
        {
            var logs = await _auditLogRepo.GetAllAsync(); // Already includes User
            return logs.Select(log => new AuditLogDto
            {
                LogId = log.LogId,
                UserId = log.UserId,
                UserEmail = log.UserId == null ? "Admin (Hệ thống)" : log.User?.AccountEmail,
                Action = log.Action,
                Entity = log.EntityType,
                BeforeData = log.BeforeData,
                AfterData = log.AfterData,
                Timestamp = log.Timestamp
            });
        }

        public async Task<AuditLogDto?> GetByIdAsync(int id)
        {
            var log = await _auditLogRepo.GetByIdAsync(id);
            if (log == null) return null;

            // Load user manually if needed
            SystemAccount? user = null;
            if (log.UserId.HasValue)
            {
                user = await _accountRepo.GetByIdAsync(log.UserId.Value);
            }

            return new AuditLogDto
            {
                LogId = log.LogId,
                UserId = log.UserId,
                UserEmail = log.UserId == null ? "Admin (Hệ thống)" : user?.AccountEmail,
                Action = log.Action,
                Entity = log.EntityType,
                BeforeData = log.BeforeData,
                AfterData = log.AfterData,
                Timestamp = log.Timestamp
            };
        }

        public async Task CreateAsync(short? userId, string action, string entity, string? beforeData, string? afterData)
        {
            var auditLog = new DataAccess.Models.AuditLog
            {
                UserId = userId,
                Action = action,
                EntityType = entity,
                BeforeData = beforeData,
                AfterData = afterData,
                Timestamp = DateTime.UtcNow
            };

            await _auditLogRepo.AddAsync(auditLog);
        }

        public async Task LogActionAsync(string action, string entityType, string? entityId, object? oldData, object? newData)
        {
            short? userId = null;
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (short.TryParse(userIdClaim, out short parsedId) && parsedId > 0)
            {
                userId = parsedId;
            }

            var auditLog = new DataAccess.Models.AuditLog
            {
                UserId = userId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                BeforeData = oldData != null ? JsonSerializer.Serialize(oldData) : null,
                AfterData = newData != null ? JsonSerializer.Serialize(newData) : null,
                Timestamp = DateTime.UtcNow
            };

            await _auditLogRepo.AddAsync(auditLog);
        }

        public async Task<IEnumerable<AuditLogDto>> FilterAsync(short? userId, string? entity, DateTime? fromDate, DateTime? toDate)
        {
            var logs = await _auditLogRepo.FilterAsync(userId, entity, fromDate, toDate);
            
            return logs.Select(log => new AuditLogDto
            {
                LogId = log.LogId,
                UserId = log.UserId,
                UserEmail = log.UserId == null ? "Admin (Hệ thống)" : log.User?.AccountEmail,
                Action = log.Action,
                Entity = log.EntityType,
                BeforeData = log.BeforeData,
                AfterData = log.AfterData,
                Timestamp = log.Timestamp
            });
        }
    }
}
