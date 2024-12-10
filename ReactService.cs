using GreenDefined.DTOs.Reacts;
using GreenDefined.Models;
using GreenDefined.Repository.IRepo;
using GreenDefined.Service.IServices;

namespace GreenDefined.Service.Services
{
    public class ReactService : IReactService
    {
        #region Fields
        private readonly IReactRepository _Reactrepository;
        private readonly INotificationTransactionService _notificationService;
        #endregion

        #region Ctor
        public ReactService(IReactRepository Reactrepository, INotificationTransactionService notificationService)
        {
            _Reactrepository = Reactrepository;
            _notificationService = notificationService;

        }
        #endregion

        #region Handle Functions
        public async Task<string> AddReact(AddReactDTO dto)
        {
            var react = new React()
            {
                PostId = dto.PostID,
                userId = dto.userID,
                UpOrDown = dto.UpOrDown,
                CreatedAt = DateTime.UtcNow,

            };
            //Get if react exist or no
            var existReact = _Reactrepository.GetTableNoTracking().Where(x => x.PostId == dto.PostID && x.userId == dto.userID).FirstOrDefault();
            if (existReact == null || react.UpOrDown != existReact.UpOrDown)
            {//new react or Different
                if (existReact != null && react.UpOrDown != existReact.UpOrDown)
                    await _Reactrepository.DeleteAsync(existReact);
                //if not exist add new react
                await _Reactrepository.AddAsync(react);
                //Add To Notifications
                var Noty = new NotificationTransaction()
                {
                    ReactId = react.id,
                    userID = react.userId,
                };
                await _notificationService.AddNoty(Noty);
                return "Success";
            }
            //WorestCase->Was Exist!
            //  if (existReact != null && existReact.UpOrDown == react.UpOrDown)

            await _Reactrepository.DeleteAsync(existReact);
            return "Deleted Saccessfuly";



        }

        public int GetPostLikesCount(int postID)
        {
            return _Reactrepository.GetTableNoTracking().Where(x => x.PostId == postID && x.UpOrDown == true).Count();
        }
        public int GetPostDisLikesCount(int postID)
        {
            return _Reactrepository.GetTableNoTracking().Where(x => x.PostId == postID && x.UpOrDown == false).Count();
        }

        public string GetPostUserStatus(int postId, string userId)
        {
            var x = _Reactrepository.GetTableNoTracking().Where(x => x.userId == userId && x.PostId == postId).FirstOrDefault();
            var reacts = _Reactrepository.GetTableAsList();
            foreach (var react in reacts)
            {
                if (react.PostId == postId && react.userId == userId)
                {
                    if (react.UpOrDown) { return "Yes"; }
                    return "No";
                }
            }
            return "None";

        }
        #endregion
    }
}
