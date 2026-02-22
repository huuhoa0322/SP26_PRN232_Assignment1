namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNewsCreatedNotificationAsync(string newsTitle, string createdByName, DateTime createdDate);
    }
}
