using Domain.DomainModel.Entity.RolesAndAccess;
using Domain.DomainModel.Interface.RolesAndAccess;
using Infrastucture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IAccessRepository _accessRepository;

        public AccessController(DataContext dataContext, IAccessRepository accessRepository)
        {
            _dataContext = dataContext;
            _accessRepository = accessRepository;
        }

        #region Get Data
        [HttpGet("GetAccesses")]
        public async Task<ActionResult<List<Access>>> GetAccesses()
        {
            return Ok(await _accessRepository.GetAccesses());
        }

        [HttpGet("GetAccessById")]
        public async Task<ActionResult<Access>> GetAccessById(int accessId)
        {
            var access = await _accessRepository.GetAccess(accessId);
            if (access == null)
                return NotFound("Access not found!");
            return Ok(access);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateAccess")]
        public async Task<ActionResult<List<Access>>> CreateAccess(Access access)
        {
            var accessList = await _accessRepository.Create(access);

            //if (accessList == null)
            //    return NotFound("New access not created!");

            return Ok(accessList);
        }

        [HttpPut("UpdateAccess")]
        public async Task<ActionResult<List<Access>>> UpdateAccess(Access access)
        {
            var accessList = await _accessRepository.Update(access);

            //if (accessList == null)
            //    return NotFound("Error encountered while updating access!");

            return Ok(accessList);
        }

        [HttpDelete("DeleteAccess")]
        public async Task<ActionResult<List<Access>>> DeleteAccess(List<int> accessIds)
        {
            var accessList = await _accessRepository.Delete(accessIds);

            //if (accessList == null)
            //    return NotFound("Error encountered when deleting access!");

            return Ok(accessList);
        }
        #endregion
    }
}
