using AutoMapper;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.AuditLog;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using FUNewsManagement_v2_CoreAPI.DataAccess.Models;
using FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces;

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

        public AuditLogService(
            IAuditLogRepository auditLogRepo,
            IAccountRepository accountRepo,
            IMapper mapper)
        {
            _auditLogRepo = auditLogRepo;
            _accountRepo = accountRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AuditLogDto>> GetAllAsync()
        {
            var logs = await _auditLogRepo.GetAllAsync(); // Already includes User
            return logs.Select(log => new AuditLogDto
            {
                LogId = log.LogId,
                UserId = log.UserId,
                UserEmail = log.User?.AccountEmail,
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
                UserEmail = user?.AccountEmail,
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

        public async Task<IEnumerable<AuditLogDto>> FilterAsync(short? userId, string? entity, DateTime? fromDate, DateTime? toDate)
        {
            var logs = await _auditLogRepo.FilterAsync(userId, entity, fromDate, toDate);
            
            return logs.Select(log => new AuditLogDto
            {
                LogId = log.LogId,
                UserId = log.UserId,
                UserEmail = log.User?.AccountEmail,
                Action = log.Action,
                Entity = log.EntityType,
                BeforeData = log.BeforeData,
                AfterData = log.AfterData,
                Timestamp = log.Timestamp
            });
        }
    }
}
