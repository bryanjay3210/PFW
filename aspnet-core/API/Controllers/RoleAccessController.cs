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
    public class RoleAccessController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IRoleAccessRepository _roleAccessRepository;

        public RoleAccessController(DataContext dataContext, IRoleAccessRepository roleAccessRepository)
        {
            _dataContext = dataContext;
            _roleAccessRepository = roleAccessRepository;
        }

        #region Get Data
        [HttpGet("GetRoleAccesses")]
        public async Task<ActionResult<List<RoleAccess>>> GetRoleAccesses()
        {
            return Ok(await _roleAccessRepository.GetRoleAccesses());
        }

        [HttpGet("GetRoleAccessById")]
        public async Task<ActionResult<RoleAccess>> GetRoleAccessById(int roleAccessId)
        {
            var roleAccess = await _roleAccessRepository.GetRoleAccess(roleAccessId);
            if (roleAccess == null)
                return NotFound("RoleAccess not found!");
            return Ok(roleAccess);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateRoleAccess")]
        public async Task<ActionResult<List<RoleAccess>>> CreateRoleAccess(RoleAccess roleAccess)
        {
            var roleAccessList = await _roleAccessRepository.Create(roleAccess);

            //if (roleAccessList == null)
            //    return NotFound("New roleAccess not created!");

            return Ok(roleAccessList);
        }

        [HttpPut("UpdateRoleAccess")]
        public async Task<ActionResult<List<RoleAccess>>> UpdateRoleAccess(RoleAccess roleAccess)
        {
            var roleAccessList = await _roleAccessRepository.Update(roleAccess);

            //if (roleAccessList == null)
            //    return NotFound("Error encountered while updating roleAccess!");

            return Ok(roleAccessList);
        }

        [HttpDelete("DeleteRoleAccess")]
        public async Task<ActionResult<List<RoleAccess>>> DeleteRoleAccess(List<int> roleAccessIds)
        {
            var roleAccessList = await _roleAccessRepository.Delete(roleAccessIds);

            //if (roleAccessList == null)
            //    return NotFound("Error encountered when deleting roleAccess!");

            return Ok(roleAccessList);
        }
        #endregion
    }
}
