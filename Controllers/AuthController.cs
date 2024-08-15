using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDTO userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExist = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" + userForRegistration.Email + "'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExist);
                if (existingUsers.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create(
                    ))
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

                    string sqlAddAuth = @"EXEC TutorialAppSchema.spRegistration_Upsert
                        @Email = @EmailParameter, 
                        @PasswordHash =  @PasswordHashParam, 
                        @PasswordSalt = @PasswordSaltParam";

                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHashParam", SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;
                    sqlParameters.Add(passwordHashParameter);

                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSaltParam", SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;
                    sqlParameters.Add(passwordSaltParameter);

                    SqlParameter emailParameter = new SqlParameter("@EmailParameter", SqlDbType.VarChar);
                    emailParameter.Value = userForRegistration.Email;
                    sqlParameters.Add(emailParameter);



                    if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                    {
                        string sqlAddUser = @"EXEC TutorialAppSchema.spUser_Upsert
                            @FirstName = '" + userForRegistration.FirstName +
                        "', @LastName = '" + userForRegistration.LastName +
                        "', @Email =  '" + userForRegistration.Email +
                        "', @Gender =  '" + userForRegistration.Gender +
                        "', @Active = 1" +
                        ",  @JobTitle = '" + userForRegistration.JobTitle +
                        "', @Department = '" + userForRegistration.Department +
                        "', @Salary = '" + userForRegistration.Salary + "'";
                        if (_dapper.ExecuteSql(sqlAddUser))
                        {
                            return Ok();
                        }
                        throw new Exception("Failed to Add User");
                    }
                    throw new Exception("Failed to Register user");
                }
                throw new Exception("User with this Email already Exist");
            }
            throw new Exception("Passwords do not match");
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDTO userForLogin)
        {

            string sqlForHashAndSalt = @"SELECT [PasswordHash],
                                    [PasswordSalt] FROM TutorialAppSchema.Auth
                                    WHERE Email = '" + userForLogin.Email + "'";
            UserForLoginConfirmationDTO userForConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmationDTO>(sqlForHashAndSalt);

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

            for (int i = 0; i < passwordHash.Length; i++)
            {
                if (passwordHash[i] != userForConfirmation.PasswordHash[i])
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Incorrect Password");
                }
            }

            string userIdSql = @"
            SELECT UserId FROM TutorialAppSchema.Users 
            WHERE Email = '" + userForLogin.Email + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string> {
                {"token", _authHelper.CreateToken(userId)}
            });
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userId = User.FindFirst("userId")?.Value + "";

            string userIdSql = "SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = "
            + userId;
            int userIdFromDB = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string> {
                {"token", _authHelper.CreateToken(userIdFromDB)}
            });
        }

    }
}