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
    DataContextEF _entity;
    public UserEFController(IConfiguration config)
    {
        _entity = new DataContextEF(config);
        _mapper = new Mapper(new MapperConfiguration(cfg => {
            cfg.CreateMap<UserToAddDto, User>();
        }));
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _entity.Users.ToList<User>();
        return users;
    }

    [HttpGet("GetSingleUser/{userId}")]
    // public IActionResult Test()
    public IActionResult GetSingleUser(int userId)
    {
        User? user = _entity.Users
        .Where(u => u.UserId == userId)
        .FirstOrDefault<User>();
        if (user != null)
        {
            return Ok(user);
        }
        return NotFound("Failed to Get User");
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userDB = _entity.Users
        .Where(u => u.UserId == user.UserId)
        .FirstOrDefault<User>();

        if (userDB != null)
        {
            userDB.Active = user.Active;
            userDB.FirstName = user.FirstName;
            userDB.LastName = user.LastName;
            userDB.Email = user.Email;
            userDB.Gender = user.Gender;
            if (_entity.SaveChanges() > 0)
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
        _entity.Add(userDB);
        if (_entity.SaveChanges() > 0)
        {
            return Ok();
        }
        return StatusCode(StatusCodes.Status500InternalServerError, "Failed to Add User");
    }


    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userDB = _entity.Users
        .Where(u => u.UserId == userId)
        .FirstOrDefault<User>();

        if (userDB != null)
        {
            _entity.Users.Remove(userDB);
            if (_entity.SaveChanges() > 0)
            {
                return Ok();
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to Delete User");
        }

        return NotFound("Failed to Get User");
    }
}

