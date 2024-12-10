using GreenDefined.DTOs.Comments;

namespace GreenDefined.Service.IServices
{
    public interface ICommentService
    {
        Task<string> AddComment(AddCommentDTO commentDTO);
    }
}
