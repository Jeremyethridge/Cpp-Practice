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

