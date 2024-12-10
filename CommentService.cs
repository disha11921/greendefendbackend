using GreenDefined.DTOs.Comments;
using GreenDefined.Models;
using GreenDefined.Repository.IRepo;
using GreenDefined.Service.IServices;

namespace GreenDefined.Service.Services
{
    public class CommentService : ICommentService
    {
        #region Fields
        private readonly ICommentRepository _commentRepo;
        private readonly INotificationTransactionService _notificationService;
        #endregion

        #region Ctor
        public CommentService(ICommentRepository commentRepo, INotificationTransactionService notificationService)
        {
            _commentRepo = commentRepo;
            _notificationService = notificationService;

        }
        #endregion

        #region Handle Functions
        public async Task<string> AddComment(AddCommentDTO commentDTO)
        {
            var comment = new Comment()
            {
                CommentValue = commentDTO.Comment,
                PostID = commentDTO.PostID,
                userId = commentDTO.userID,
                CreatedAt = DateTime.UtcNow,
            };
            await _commentRepo.AddAsync(comment);
            //Add To Notifications
            var Noty = new NotificationTransaction()
            {
                CommentId = comment.id,
                userID = commentDTO.userID,
            };
            await _notificationService.AddNoty(Noty);
            return "";
        }

        #endregion
    }
}
