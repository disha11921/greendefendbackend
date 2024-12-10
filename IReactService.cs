using GreenDefined.DTOs.Reacts;

namespace GreenDefined.Service.IServices
{
    public interface IReactService
    {
        Task<string> AddReact(AddReactDTO dto);
        int GetPostLikesCount(int postID);
        int GetPostDisLikesCount(int postID);
        string GetPostUserStatus(int postId, string userId);
    }
}
