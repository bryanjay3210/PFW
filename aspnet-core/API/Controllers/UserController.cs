using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Interface.User;
using Infrastucture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly string _creditMemoFilterDate;

        public UserController(DataContext dataContext, IUserRepository userRepository, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _userRepository = userRepository;
            _configuration = configuration;
            _creditMemoFilterDate = _configuration.GetValue<string>("CreditMemoFilterDate");
        }

        #region Get Data
        [HttpGet("GetUsers")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            return Ok(await _userRepository.GetUsers());
        }

        [HttpGet("GetUserById")]
        public async Task<ActionResult<User>> GetUserById(int userId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
                return NotFound("User not found!");
            return Ok(user);
        }

        [HttpGet("GetUserByEmail")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
                return NotFound("User not found!");
            return Ok(user);
        }

        [HttpGet("GetUserNotificationsByUserId")]
        public async Task<ActionResult<UserNotificationDTO>> GetUserNotificationsByUserId(int userId)
        {
            var userNotification = await _userRepository.GetUserNotificationsByUserId(userId, _creditMemoFilterDate);
            return Ok(userNotification);
        }

        [HttpGet("GetSessionTimeout")]
        public ActionResult<SessionTimeoutDTO> GetSessionTimeout()
        {
            SessionTimeoutDTO result = new SessionTimeoutDTO()
            { 
                DialogCountdown = int.Parse(_configuration.GetSection("AppSettings:DialogCountdown").Value),
                SessionTimeout = int.Parse(_configuration.GetSection("AppSettings:SessionTimeout").Value)
            };

            return Ok(result);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateUser")]
        public async Task<ActionResult<List<User>>> CreateUser(User user)
        {
            var userList = await _userRepository.Create(user);

            //if (userList == null)
            //    return NotFound("New user not created!");

            return Ok(userList);
        }

        [HttpPut("UpdateUser")]
        public async Task<ActionResult<List<User>>> UpdateUser(User user)
        {
            var userList = await _userRepository.Update(user);

            //if (userList == null)
            //    return NotFound("Error encountered while updating user!");

            return Ok(userList);
        }

        [HttpDelete("DeleteUser")]
        public async Task<ActionResult<List<User>>> DeleteUser(List<int> userIds)
        {
            var userList = await _userRepository.Delete(userIds);

            //if (userList == null)
            //    return NotFound("Error encountered when deleting user!");

            return Ok(userList);
        }
        #endregion
    }
}
