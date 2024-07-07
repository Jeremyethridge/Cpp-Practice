using System.Data;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
        }

        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDTO userForRegister)
        {
            if (userForRegister.Password == userForRegister.PasswordConfirm)
            {
                string sqlCheckUserExist = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" + userForRegister.Email + "'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExist);
                if (existingUsers.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

                    byte[] passwordHash = KeyDerivation.Pbkdf2(
                        password: userForRegister.Password,
                        salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8
                    );

                    string sqlAddAuth = @"INSERT INTO TutorialAppSchema.Auth ([Email],
                                    [PasswordHash],
                                    [PasswordSalt]) Values ('" + userForRegister.Email + 
                                    "', @PasswordHash, @PasswordSalt)";

                                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                                    passwordSaltParameter.Value = passwordSalt;

                                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                                    passwordHashParameter.Value = passwordHash;

                                    sqlParameters.Add(passwordSaltParameter);
                                    sqlParameters.Add(passwordHashParameter);

                                    if(_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                                    {
                                                            return Ok();
                                    }
                                    throw new Exception("Failed to Register user");
                                    }
                throw new Exception("User with this Email already Exist");
            }
                throw new Exception("Passwords do not match");
            }

            [HttpPost("Login")]
            public IActionResult Login(UserForLoginConfirmationDTO userForLogin)
            {
                return Ok();
            }
        }
    }