using System.ComponentModel.DataAnnotations;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account
{
    /// <summary>
    /// DTO cho SystemAccount
    /// </summary>
    public class AccountDto
    {
        [Key]
        public short AccountId { get; set; }
        public string AccountName { get; set; } = null!;
        public string AccountEmail { get; set; } = null!;
        public short? AccountRole { get; set; } 
    }
}
