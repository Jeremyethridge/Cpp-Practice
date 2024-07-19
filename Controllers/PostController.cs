using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[Controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public PostController (IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("Posts")]
        public IEnumerable<Post> GetPosts()
        {
            string sql = @"
                        SELECT [PostId],
                        [UserId],
                        [PostTitle],
                        [PostContent],
                        [PostCreated],
                        [PostUpdated] FROM TutorialAppSchema.Posts";
               return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("PostSingle/{postId}")]
        public IEnumerable<Post> GetSinglePost(int postId)
        {
            string sql = @"
                        SELECT [PostId],
                        [UserId],
                        [PostTitle],
                        [PostContent],
                        [PostCreated],
                        [PostUpdated] FROM TutorialAppSchema.Posts
                        WHERE PostId = " + postId.ToString();
            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("PostsByUser/{userId}")]
        public IEnumerable<Post> PostsByUser(int userId)
        {
            string sql = @"
                        SELECT [PostId],
                        [UserId],
                        [PostTitle],
                        [PostContent],
                        [PostCreated],
                        [PostUpdated] FROM TutorialAppSchema.Posts
                        WHERE UserId = " + userId.ToString();
            return _dapper.LoadData<Post>(sql);
        }
        [HttpGet("MyPost/{userId}")]
        public IEnumerable<Post> GetMyPost()
        {
            string sql = @"
                        SELECT [PostId],
                        [UserId],
                        [PostTitle],
                        [PostContent],
                        [PostCreated],
                        [PostUpdated] FROM TutorialAppSchema.Posts
                        WHERE UserId = " + User.FindFirst("userId")?.Value;
            return _dapper.LoadData<Post>(sql);
        }
    }
}