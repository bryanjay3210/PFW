using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;

        public AuthController(IConfiguration configuration, IUserService userService, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userService = userService;
            _userRepository = userRepository;
        }

        [HttpGet, Authorize]
        public ActionResult<object> GetMe()
        {
            var userName = _userService.GetUserName();
            var role = User?.FindFirstValue(ClaimTypes.Role);
            return Ok( new { userName, role} );
        }


        [HttpGet("RequestToken")]    
        public async Task<ActionResult<string>> RequestToken()
        {
            var result = string.Empty;

            //NJPR
            //var user = _userRepository.GetUserByEmail("demo@user.com").Result;
            var user = _userRepository.GetUserByEmail("carparts@perfectfitwest.com").Result;
            if (user != null)
            {
                //NJPR
                //if (!VerifyPasswordHash("demouser", user.PasswordHash, user.PasswordSalt))
                if (!VerifyPasswordHash("car90755", user.PasswordHash, user.PasswordSalt))
                {
                    result = "Invalid Password!";
                    return BadRequest(result);
                }

                result = CreateToken(user);
                return result;
            }
            else
            {
                return BadRequest("User not found.");
            }
        }


        [HttpPost("Register")]
        public async Task<ActionResult<RegistrationResponse>> Register (UserDTO userDTO)
        {
            var result = new RegistrationResponse() { Status = 400 };

            try
            {
                var userResult = new Domain.DomainModel.Entity.User();

                CreatePasswordHash(userDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);

                userResult.UserName = userDTO.UserName;
                userResult.Email = userDTO.Email;
                userResult.RoleId = userDTO.RoleId;
                userResult.IsCustomerUser = userDTO.IsCustomerUser;
                userResult.CustomerId = userDTO.CustomerId;
                userResult.LocationId = userDTO.LocationId;
                userResult.IsActivated = userDTO.IsActivated;
                userResult.IsChangePasswordOnLogin = userDTO.IsChangePasswordOnLogin;
                userResult.CreatedBy = userDTO.CreatedBy;
                userResult.CreatedDate = userDTO.CreatedDate;
                userResult.IsActive = userDTO.IsActive;
                userResult.IsDeleted = false;
                userResult.PasswordHash = passwordHash;
                userResult.PasswordSalt = passwordSalt;
                userResult = await _userRepository.Create(userResult);

                result.Message = "Registration Successful";
                result.Status = 200;
                result.User = userResult;
                result.Users = _userRepository.GetUsers().Result;
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Message = "Registration Failed: " + e.Message;
                return Ok(result);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginDTO loginDTO)
        {
            var result = new LoginResponse() { Status = 400 };

            try
            {
                var user = _userRepository.GetUserByEmail(loginDTO.Email).Result;

                if (user == null || user.Email != loginDTO.Email)
                {
                    result.Message = "User not found!";
                    return BadRequest(result);
                }

                if (!user.IsActivated)
                {
                    result.Message = "User is not yet activated!";
                    return BadRequest(result);
                }

                if (user.IsDeleted)
                {
                    result.Message = "User is deleted!";
                    return BadRequest(result);
                }

                if (!user.IsActive)
                {
                    result.Message = "User is deactivated!";
                    return BadRequest(result);
                }

                if (!VerifyPasswordHash(loginDTO.Password, user.PasswordHash, user.PasswordSalt))
                {
                    result.Message = "Invalid Password!";
                    return BadRequest(result);
                }

                string token = CreateToken(user);

                result.Status = 200;
                result.Message = "Login Successful";
                result.Token = token;
                result.User = user;

                return Ok(result);
            }
            catch (Exception e)
            {
                result.Message = "Login Failed: " + e.Message;
                return Ok(result);
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            
            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
