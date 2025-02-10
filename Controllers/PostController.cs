using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[Controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
        public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "None")
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get";
            string stringParameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            if (postId != 0)
            {
                stringParameters += ", @PostId=@PostIdParameter";
                sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);
            }
            if (userId != 0)
            {
                stringParameters = ", @UserId=@UserIdParameter";
                sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }
            if (searchParam.ToLower() != "none")
            {
                stringParameters += ", @searchValue=@SearchParameter";
                sqlParameters.Add("@SearchParameter", searchParam, DbType.String);
            }

            if (stringParameters.Length > 0)
            {
                sql += stringParameters.Substring(1);
            }
            return _dapper.LoadDataWithParams<Post>(sql, sqlParameters);
        }

        [HttpGet("MyPost/{userId}")]
        public IEnumerable<Post> GetMyPost()
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId=@UserIdParameter";
            DynamicParameters sqlParameter = new DynamicParameters();
            sqlParameter.Add("UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);
            return _dapper.LoadDataWithParams<Post>(sql, sqlParameter);
        }

        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post postToUpsert)
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
                                @UserId=@UserIdParameter,
                                @PostTitle=@PostTitleParameter,
                                @PostContent=@PostContentParameter";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);
            sqlParameters.Add("@PostTitleParameter", postToUpsert.PostTitle, DbType.String);
            sqlParameters.Add("@PostContentParameter", postToUpsert.PostContent, DbType.String);

            if (postToUpsert.PostId > 0)
            {
                sql += ", @PostId=@PostIdParameter";
                sqlParameters.Add("@PostIdParameter", postToUpsert.PostId, DbType.Int32);
            }

            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
            {
                return Ok();
            }
            throw new Exception("Failed to create new post!");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Delete 
            @PostId = @PostIdParameter
            @UserId = @UserIdParameter";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);
            sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);

            if(_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
            {
                return Ok();
            }
            throw new Exception("Failed to Delete Post");
        }
    }
}
