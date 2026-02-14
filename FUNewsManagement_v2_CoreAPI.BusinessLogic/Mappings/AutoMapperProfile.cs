using AutoMapper;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account;
using FUNewsManagement_v2_CoreAPI.DataAccess.Models;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Mappings
{
    /// <summary>
    /// AutoMapper profile cho mapping giữa entities và DTOs
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // SystemAccount <-> AccountDto
            CreateMap<SystemAccount, AccountDto>(); 
            CreateMap<AccountDto, SystemAccount>();
        }
    }
}
