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

        [HttpGet("GetSalary")]
        public IActionResult GetSalary()
        {
            try
            {
                string sql = @"
    SELECT [UserId],
        [Salary],
        [AverageSalary] 
        FROM TutorialAppSchema.UserSalary;
    ";
                IEnumerable<UserSalary> salaries = _dapper.LoadData<UserSalary>(sql);
                if (salaries == null)
                {
                    return NotFound("No Salaries Found");
                }
                return Ok(salaries);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving Salaries: {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please try again later");
            }
        }

        [HttpGet("GetSingleSalary/userId")]
        public IActionResult GetSingleSalary(int userId)
        {
            try
            {
                string sql = @"
                SELECT [UserId],
                    [Salary],
                    [AverageSalary] FROM TutorialAppSchema.UserSalary
                Where UserId = " + userId.ToString();
                UserSalary salary = _dapper.LoadDataSingle<UserSalary>(sql);
                if (salary == null)
                {
                    return NotFound("No Salary is Found");
                }
                return Ok(salary);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving Salaries: {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please try again later");
            }
        }
        //Edit user by changing field 
        [HttpPut("EditSalary")]
        public IActionResult EditSalary(UserSalary userSalary)
        {
            try
            {
                string sql = @"
                UPDATE TutorialAppSchema.UserSalary
                    SET [Salary] = '" + userSalary.Salary +
                 "', [AverageSalary] = '" + userSalary.AverageSalary +
                 "' WHERE UserId = " + userSalary.UserId;
                if (_dapper.ExecuteSql(sql))
                {
                    return Ok();
                }
                else
                {
                    return NotFound("No Salary is Found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Updating Salaries: {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please try again later");
            }
        }
        // Deleting by ID
        [HttpPost("AddSalary")]
        public IActionResult AddSalary(UserSalaryToAddDto UserSalary)
        {
            try
            {
                string sql = @"
             INSERT INTO TutorialAppSchema.UserSalary(
                [Salary],
                [AverageSalary]
             ) VALUES (" +
                "'" + UserSalary.Salary +
                    "', '" + UserSalary.AverageSalary +
    "')";       
                if (_dapper.ExecuteSql(sql))
                {
                    return Ok();
                }
                return NotFound("Failed to Add Salary");
            }
            catch (Exception ex) { 
                Console.WriteLine($"Error Getting Salary: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please try again later");
            }
        }
    }
}