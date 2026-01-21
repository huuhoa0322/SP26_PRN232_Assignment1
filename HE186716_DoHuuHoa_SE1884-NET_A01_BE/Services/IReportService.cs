using HE186716_DoHuuHoa_SE1884_NET_A01_BE.DTOs;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_BE.Services;

public interface IReportService
{
    Task<ReportStatisticsDto> GetStatisticsAsync(ReportFilterDto filter);
    Task<byte[]> ExportToCsvAsync(ReportFilterDto filter);
}
