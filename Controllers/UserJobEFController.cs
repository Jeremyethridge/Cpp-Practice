using System.Data;
using System.Data;
using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;
[ApiController]
[Route("[controller]")]
public class UserJobEFController : ControllerBase
{
    IMapper _mapper;
    IUserRepository _userRepository;
    public UserJobEFController(IConfiguration config, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _mapper = new Mapper(new MapperConfiguration(cfg => {
            cfg.CreateMap<UserJobInfoToAddDto, UserJobInfo>();
        }));
    }

    [HttpGet("GetUserJobs")]
    public IEnumerable<UserJobInfo> GetUserJobs()
    {
        IEnumerable<UserJobInfo> userJobs = _userRepository.GetUserJobs();
        return userJobs;
    }

    [HttpGet("GetSingleJobEF")]
    // public IActionResult Test()
    public UserJobInfo GetSingleJob(int userId)
    {
       return  _userRepository.GetSingleUserJob(userId);
    }

    [HttpPut("EditUserEF")]
    public IActionResult EditUser(UserJobInfo job)
    {
        UserJobInfo? userJob = _userRepository.GetSingleUserJob(job.UserId);

        if (userJob != null)
        {
            _mapper.Map(userJob, job);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to Update User");
        }
        return NotFound("Failed to Get User");
    }

    [HttpPost("AddUserEF")]
    public IActionResult AddUser(UserJobInfoToAddDto job)
    {
        UserJobInfo userJob = _mapper.Map<UserJobInfo>(job);
        _userRepository.AddEntity<UserJobInfo>(userJob);
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        return StatusCode(StatusCodes.Status500InternalServerError, "Failed to Add User");
    }


    [HttpDelete("DeleteUserJobEF/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        UserJobInfo? userJob = _userRepository.GetSingleUserJob(userId);

        if (userJob != null)
        {
            _userRepository.RemoveEntity<UserJobInfo>(userJob);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to Delete User");
        }

        return NotFound("Failed to Get User");
    }
}

