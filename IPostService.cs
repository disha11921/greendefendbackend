using GreenDefined.DTOs.Posts;

namespace GreenDefined.Service.IServices
{
    public interface IPostService
    {
        Task<string> AddPost(AddPostDTO dto);
        Task<List<GetPostsDTO>> GetPosts(string userid);
        Task<GetPostDetailDTO> GetPostDetail(int PostId);
    }
}
