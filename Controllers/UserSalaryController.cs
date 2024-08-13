using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{

    // Creating new controller endpoints for Salary and JobInfo
    // That connect to tables using EF and Dapper
    [ApiController]
    [Route("[Controller]")]

    public class UserSalaryController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public UserSalaryController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        } 
                
        [HttpDelete("DeleteSalary/{userId}")]
        // Grab user id and delete salary by id
        public IActionResult DeleteSalary(int userId)
        {
            try
            {
                string sql = @"
                DELETE
                    FROM TutorialAppSchema.UserSalary
                WHERE UserId = " + userId.ToString();
                if (_dapper.ExecuteSql(sql))
                {
                    return Ok();
                }
                return NotFound("Failed to Delete Salary");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Getting Salary: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please try again later");
            }
        }
    }
}