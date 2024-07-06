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
public class UserEFController : ControllerBase
{
    IMapper _mapper;
    IUserRepository _userRepository;
    public UserEFController(IConfiguration config, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _mapper = new Mapper(new MapperConfiguration(cfg => {
            cfg.CreateMap<UserToAddDto, User>();
        }));
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _userRepository.GetUsers();
        return users;
    }

    [HttpGet("GetSingleUser/{userId}")]
    // public IActionResult Test()
     public User GetSingleUser(int userId)
    {
        return _userRepository.GetSingleUser(userId);
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userDB = _userRepository.GetSingleUser(user.UserId);

        if (userDB != null)
        {
            userDB.Active = user.Active;
            userDB.FirstName = user.FirstName;
            userDB.LastName = user.LastName;
            userDB.Email = user.Email;
            userDB.Gender = user.Gender;
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to Update User");
        }
        return NotFound("Failed to Get User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        User userDB = _mapper.Map<User>(user);
        _userRepository.AddEntity<User>(userDB);
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        return StatusCode(StatusCodes.Status500InternalServerError, "Failed to Add User");
    }


    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userDB = _userRepository.GetSingleUser(userId);

        if (userDB != null)
        {
            _userRepository.RemoveEntity<User>(userDB);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to Delete User");
        }

        return NotFound("Failed to Get User");
    }
}

