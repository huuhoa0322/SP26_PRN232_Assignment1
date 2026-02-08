using System;
using System.Collections.Generic;

namespace FUNewsManagement_v2_CoreAPI.DataAccess.Models;

public partial class RefreshToken
{
    public int TokenId { get; set; }

    public string Token { get; set; } = null!;

    public short AccountId { get; set; }

    public DateTime ExpiryDate { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual SystemAccount Account { get; set; } = null!;
}
