using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace DotnetAPI.Controllers;
[ApiController]
[Route("[Controller]")]

public class UserSalaryEFController : ControllerBase
{
    IMapper _mapper;
    DataContextEF _entity;
    public UserSalaryEFController(IConfiguration config)
    {
        _entity = new DataContextEF(config)
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserSalaryToAddDto, UserSalary>();
        }));
    }
    [HttpGet("GetSalariesEF")]
    public IEnumerable<UserSalary> GetSalaries()
    {
        IEnumerable<UserSalary> salaries = _entity.UserSalary.ToList<UserSalary>();
        return salaries;
    }
    [HttpGet("GetSingleSalaryEF")]
    public IActionResult GetSingleSalary(int userId)
    {
        UserSalary? salary = _entity.UserSalary
        .Where(u => u.UserId == userId)
        .FirstOrDefault<UserSalary>();
        if (salary != null)
        {
            return Ok(salary);
        }
        return NotFound("Failed to get salary");
    }

    [HttpPut("EditSalaryEF")]
    public IActionResult EditSalaryEF(UserSalary salary)
    {
        UserSalary? editSalary = _entity.UserSalary
        .Where(u => u.UserId == salary.UserId)
        .FirstOrDefault<UserSalary>();
        if (editSalary != null)
        {
            editSalary.Salary = salary.Salary;
            editSalary.AverageSalary = salary.AverageSalary;

            if (_entity.SaveChanges() > 0)
            {
                return Ok();
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please try again later");
        }
        return NotFound("Failed find Salary");
    }
    [HttpPost("AddSalaryEF")]
    public IActionResult AddSalary(UserSalaryToAddDto salary)
    {
        UserSalary userSalary = _mapper.Map<UserSalary>(salary);
        _entity.Add(salary);
        if (_entity.SaveChanges() > 0)
        {
            return Ok();
        }
        return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please try again later");
    }

    [HttpDelete("DeleteSalaryEF/{userId}")]
    public IActionResult DeleteSalary(int userId)
    {
        UserSalary? userSalary = _entity.UserSalary
        .Where(u => u.UserId == userId)
        .FirstOrDefault<UserSalary>();

        if(userSalary != null)
        {
            _entity.Remove(userSalary);
            if(_entity.SaveChanges() > 0)
            {
                return Ok();
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please try again later.");
        }
        return NotFound("Failed to Delete Salary");
    }
}