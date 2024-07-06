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
    DataContextEF _entity;
    public UserJobEFController(IConfiguration config)
    {
        _entity = new DataContextEF(config);
        _mapper = new Mapper(new MapperConfiguration(cfg => {
            cfg.CreateMap<UserJobInfoToAddDto, UserJobInfo>();
        }));
    }

    [HttpGet("GetUsersEF")]
    public IEnumerable<UserJobInfo> GetUsers()
    {
        IEnumerable<UserJobInfo> userJobs = _entity.UserJobInfo.ToList<UserJobInfo>();
        return userJobs;
    }

    [HttpGet("GetSingleJobEF")]
    // public IActionResult Test()
    public IActionResult GetSingleJob(int userId)
    {
        UserJobInfo? job = _entity.UserJobInfo
        .Where(u => u.UserId == userId)
        .FirstOrDefault<UserJobInfo>();
        if (job != null)
        {
            return Ok(job);
        }
        return NotFound("Failed to Get User");
    }

    [HttpPut("EditUserEF")]
    public IActionResult EditUser(UserJobInfo job)
    {
        UserJobInfo? userJob = _entity.UserJobInfo
        .Where(u => u.UserId == job.UserId)
        .FirstOrDefault<UserJobInfo>();

        if (userJob != null)
        {
            _mapper.Map(userJob, job);
            // userJob.JobTitle = job.JobTitle;
            // userJob.Department = job.Department;
            if (_entity.SaveChanges() > 0)
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
        _entity.Add(userJob);
        if (_entity.SaveChanges() > 0)
        {
            return Ok();
        }
        return StatusCode(StatusCodes.Status500InternalServerError, "Failed to Add User");
    }


    [HttpDelete("DeleteUserEF/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        UserJobInfo? userJob = _entity.UserJobInfo
        .Where(u => u.UserId == userId)
        .FirstOrDefault<UserJobInfo>();

        if (userJob != null)
        {
            _entity.UserJobInfo.Remove(userJob);
            if (_entity.SaveChanges() > 0)
            {
                return Ok();
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to Delete User");
        }

        return NotFound("Failed to Get User");
    }
}

