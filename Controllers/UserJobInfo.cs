using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[Controller]")]

    public class UserJobSalaryController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public UserJobSalaryController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpPost("AddJob")]
        public IActionResult AddJob(UserJobInfoToAddDto job)
        {
            try
            {
                string sql = @"
                INSERT INTO TutorialAppSchema.UserJobInfo(
                        [JobTitle],
                        [Department]) VALUES(" + "'" + job.Department +
                        "', '" + job.JobTitle + "')";
                if (_dapper.ExecuteSql(sql))
                {
                    return Ok();
                }
                return NotFound("Failed to add Job");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting job, {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please try again later");
            }
        }
        //Update - Update Table and Update New Values of Rows
        // Fanilly Specifiy Where
        [HttpPut("EditJob")]
        public IActionResult EditJob(UserJobInfo userJob)
        {
            try 
            {
                string sql = @"
                    UPDATE TutorialAppSchema.UserJobInfo
                    SET [JobTitle] = '" + userJob.JobTitle + 
                    "', [Department] = '" + userJob.Department + 
                    "' WHERE UserId = " + userJob.UserId; 
                    if (_dapper.ExecuteSql(sql))
                    {
                        return Ok();
                    }
                    return NotFound("Job not found");
            } catch (Exception ex)
            {
                Console.WriteLine($"Error when going to edit job, {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please try again later.");
            }
        }
        [HttpDelete("DeleteJob")]
        public IActionResult DeleteJob(int userId)
        {
            try 
            {
                string sql = @"
                    DELETE TutorialAppSchema.UserJobInfo
                    WHERE UserId = " + userId.ToString();
                if(_dapper.ExecuteSql(sql))
                {
                    return Ok();
                }
                return NotFound("Job Not Found");
            } catch (Exception ex)
            {
                Console.WriteLine($"Error when going to edit job, {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please try again later.");
            }
        }
    }
}