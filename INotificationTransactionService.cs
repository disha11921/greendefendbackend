using GreenDefined.Models;

namespace GreenDefined.Service.IServices
{
    public interface INotificationTransactionService
    {
        Task<string> AddNoty(NotificationTransaction transaction);
    }
}
