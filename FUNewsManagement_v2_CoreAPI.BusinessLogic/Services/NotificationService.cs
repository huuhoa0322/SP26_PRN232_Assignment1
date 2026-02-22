using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNewsCreatedNotificationAsync(string newsTitle, string createdByName, DateTime createdDate)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", new 
            {
                NewsTitle = newsTitle,
                CreatedByName = createdByName,
                CreatedDate = createdDate
            });
        }
    }
}
