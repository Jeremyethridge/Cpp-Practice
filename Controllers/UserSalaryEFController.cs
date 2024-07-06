using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace DotnetAPI.Controllers
{


    [ApiController]
    [Route("[Controller]")]

    public class UserSalaryEFController : ControllerBase
    {
        IMapper _mapper;
        IUserRepository _userRepository;
        public UserSalaryEFController(IConfiguration config, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserSalaryToAddDto, UserSalary>();
            }));
        }
        
        [HttpGet("GetSalariesEF")]
        public IEnumerable<UserSalary> GetSalaries()
        {
            IEnumerable<UserSalary> salaries = _userRepository.GetUserSalaries();
            return salaries;
        }

        [HttpGet("GetSingleSalaryEF/{userId}")]
        public UserSalary GetSingleSalary(int userId)
        {
            return _userRepository.GetSingleUserSalary(userId);
        }

        [HttpPut("EditSalaryEF")]
        public IActionResult EditSalaryEF(UserSalary salary)
        {
            UserSalary? editSalary = _userRepository.GetSingleUserSalary(salary.UserId);
            if (editSalary != null)
            {
                _mapper.Map(salary, editSalary);
                if (_userRepository.SaveChanges())
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
        _userRepository.AddEntity<UserSalary>(userSalary);
        if (_userRepository.SaveChanges())
        {
            return Ok();
    }
        return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please try again later");
}

[HttpDelete("DeleteSalaryEF/{userId}")]
public IActionResult DeleteSalary(int userId)
{
    UserSalary userSalary = _userRepository.GetSingleUserSalary(userId);
    if (userSalary != null)
    {
        _userRepository.RemoveEntity<UserSalary>(userSalary);
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete salary");
    }
    return NotFound("Salary not found");
}
        }
}
