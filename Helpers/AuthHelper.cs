using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers 
{
    public class AuthHelper
    {
        private readonly IConfiguration _config;
        private readonly DataContextDapper _dapper;
        
        public AuthHelper(IConfiguration config)
        {
            _config = config;
            _dapper = new DataContextDapper(config);
        }
        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 1000,
            numBytesRequested: 256 / 8
        );
        }

        public string CreateToken(int userId)
        {
            Claim[] claims = new Claim[]
            {
                new Claim("userId", userId.ToString())
            };


            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;
            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    tokenKeyString != null ? tokenKeyString : "234123901u9305jfifpiofqwefijfsdflkxcvx234123901u9305jfifpiofqwefijfsdflkxcvxxcvx"
                )
            );
                 SigningCredentials credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);

                 SecurityTokenDescriptor descripter = new SecurityTokenDescriptor()
                 {
                    Subject = new ClaimsIdentity(claims),
                    SigningCredentials = credentials,
                    Expires = DateTime.Now.AddDays(1)
                 };

                 JwtSecurityTokenHandler Tokenhandler = new JwtSecurityTokenHandler();

                 SecurityToken token = Tokenhandler.CreateToken(descripter);

                 return Tokenhandler.WriteToken(token);
        }

        public bool Setpassword(UserForLoginDTO userForSetPassword)
        {
            
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create(
                    ))
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = GetPasswordHash(userForSetPassword.Password, passwordSalt);

                    string sqlAddAuth = @"EXEC TutorialAppSchema.spRegistration_Upsert
                        @Email = @EmailParameter, 
                        @PasswordHash =  @PasswordHashParam, 
                        @PasswordSalt = @PasswordSaltParam";
                    

                    DynamicParameters sqlParameters = new DynamicParameters();

                    sqlParameters.Add("@EmailParameter", userForSetPassword.Email, DbType.String);
                    sqlParameters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
                    sqlParameters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);



                    return _dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters);
        }
    } 
}