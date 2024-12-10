using GreenDefined.Models;
using GreenDefined.Repository.IRepo;
using GreenDefined.Service.IServices;

namespace GreenDefined.Service.Services
{
    public class NotificationTransactionService : INotificationTransactionService
    {
        #region Fields
        private readonly INotificationTransactionRepository _Repo;
        #endregion
        #region Ctor
        public NotificationTransactionService(INotificationTransactionRepository Repo)
        {
            _Repo = Repo;
        }
        #endregion
        #region Handle Functions
        public async Task<string> AddNoty(NotificationTransaction transaction)
        {
            await _Repo.AddAsync(transaction);
            return "";
        }
        #endregion


    }
}
