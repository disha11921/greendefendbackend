using GreenDefined.DTOs.Posts;
using GreenDefined.Models;
using GreenDefined.Repository.IRepo;
using GreenDefined.Service.IServices;
using Microsoft.EntityFrameworkCore;

namespace GreenDefined.Service.Services
{
    public class PostService : IPostService
    {
        #region Fields
        private readonly IPostRepository _postRepository;
        private readonly IUploadingImage _uploadingImage;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthService _auth;
        private readonly IReactService _reactService;
        #endregion

        #region Ctor
        public PostService(IPostRepository postRepository, IUploadingImage uploadingImage
            , IHttpContextAccessor httpContextAccessor, IAuthService auth, IReactService reactService)
        {
            _postRepository = postRepository;
            _uploadingImage = uploadingImage;
            _httpContextAccessor = httpContextAccessor;
            _auth = auth;
            _reactService = reactService;
        }
        #endregion

        #region Handle Functions
        public async Task<string> AddPost(AddPostDTO dto)
        {
            //Save image 
            var schema = _httpContextAccessor.HttpContext.Request.Scheme;
            var host = _httpContextAccessor.HttpContext.Request.Host;
            if (dto.PostImage != null)
            {
                var image = await _uploadingImage.SavingImage(dto.PostImage, schema, host);
                if (image.IsSuccess)
                {
                    var post = new Post()
                    {
                        imageUrl = image.urlOrError,
                        PostValue = dto.PostValue,
                        userId = dto.UserId,
                        CreatedAt = DateTime.UtcNow,

                    };
                    await _postRepository.AddAsync(post);
                    return "Success";
                }
                return "Faild in Saving image !";

            }
            else
            {
                var post = new Post()
                {
                    PostValue = dto.PostValue,
                    userId = dto.UserId,
                    CreatedAt = DateTime.UtcNow,
                };
                await _postRepository.AddAsync(post);
                return "Success";
            }




        }

        public async Task<List<GetPostsDTO>> GetPosts(string userid)
        {
            var allposts = _postRepository.GetTableNoTracking().Include(x => x.Comments).Include(l=>l.React).ToList();
            var response = new List<GetPostsDTO>();
            foreach (var post in allposts)
            {
                var res = new GetPostsDTO();
                var user = await _auth.GetUserById(post.userId);
                res.CommentsCount = post.Comments.Count();
                res.userName = user.FullName;
                res.userImageURL = user.imageUrl;
                res.postImageURL = post.imageUrl;
                res.postId = post.Id;
                res.postValue = post.PostValue;
                res.CreatedAt = post.CreatedAt;
               res.LikeStatus=_reactService.GetPostUserStatus(post.Id, userid);


                response.Add(res);
            }
            return response;
        }

        public async Task<GetPostDetailDTO> GetPostDetail(int PostId)
        {
            var Post = _postRepository.GetTableNoTracking().Include(x => x.Comments).Include(x => x.User).Where(x => x.Id == PostId).FirstOrDefault();


            var response = new GetPostDetailDTO()
            {
                LikesCount = _reactService.GetPostLikesCount(PostId),
                DisLikesCount = _reactService.GetPostDisLikesCount(PostId),
                PostBody = Post.PostValue,
                PostDate = Post.CreatedAt,
                PostId = Post.Id,
                PostImageURL = Post.imageUrl,
                UserCountry = Post.User.Country,
                UserImageURL = Post.User.imageUrl,
                UserName = Post.User.FullName,
                //Comment?
            };
            var Comment = new List<PostCommentsDTO>();
            foreach (var comment in Post.Comments)
            {
                var temp = new PostCommentsDTO();
                temp.CommentText = comment.CommentValue;
                temp.CommentID = comment.id;
                temp.CommentDate = comment.CreatedAt;
                //user?
                var user = await _auth.GetUserById(comment.userId);
                temp.UserName = user.FullName;
                temp.UserImageUrl = user.imageUrl;
                temp.UserCounrty = user.Country;
                //Seeding 
                Comment.Add(temp);
            }
            response.Comments = Comment;
            return response;
        }
        #endregion
    }
}
