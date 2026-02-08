using System;
using System.Collections.Generic;

namespace FUNewsManagement_v2_CoreAPI.DataAccess.Models;

public partial class AuditLog
{
    public int LogId { get; set; }

    public short? UserId { get; set; }

    public string Action { get; set; } = null!;

    public string EntityType { get; set; } = null!;

    public string? EntityId { get; set; }

    public string? BeforeData { get; set; }

    public string? AfterData { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual SystemAccount? User { get; set; }
}
