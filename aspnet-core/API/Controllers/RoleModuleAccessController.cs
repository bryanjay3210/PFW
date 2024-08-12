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
    public class RoleModuleAccessController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IRoleModuleAccessRepository _roleModuleAccessRepository;

        public RoleModuleAccessController(DataContext dataContext, IRoleModuleAccessRepository roleModuleAccessRepository)
        {
            _dataContext = dataContext;
            _roleModuleAccessRepository = roleModuleAccessRepository;
        }

        #region Get Data
        [HttpGet("GetRoleModuleAccesses")]
        public async Task<ActionResult<List<RoleModuleAccess>>> GetRoleModuleAccesses()
        {
            return Ok(await _roleModuleAccessRepository.GetRoleModuleAccesses());
        }

        [HttpGet("GetRoleModuleAccessById")]
        public async Task<ActionResult<RoleModuleAccess>> GetRoleModuleAccessById(int roleModuleAccessId)
        {
            var roleModuleAccess = await _roleModuleAccessRepository.GetRoleModuleAccess(roleModuleAccessId);
            if (roleModuleAccess == null)
                return NotFound("RoleModuleAccess not found!");
            return Ok(roleModuleAccess);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateRoleModuleAccess")]
        public async Task<ActionResult<List<RoleModuleAccess>>> CreateRoleModuleAccess(RoleModuleAccess roleModuleAccess)
        {
            var roleModuleAccessList = await _roleModuleAccessRepository.Create(roleModuleAccess);

            //if (roleModuleAccessList == null)
            //    return NotFound("New roleModuleAccess not created!");

            return Ok(roleModuleAccessList);
        }

        [HttpPut("UpdateRoleModuleAccess")]
        public async Task<ActionResult<List<RoleModuleAccess>>> UpdateRoleModuleAccess(RoleModuleAccess roleModuleAccess)
        {
            var roleModuleAccessList = await _roleModuleAccessRepository.Update(roleModuleAccess);

            //if (roleModuleAccessList == null)
            //    return NotFound("Error encountered while updating roleModuleAccess!");

            return Ok(roleModuleAccessList);
        }

        [HttpDelete("DeleteRoleModuleAccess")]
        public async Task<ActionResult<List<RoleModuleAccess>>> DeleteRoleModuleAccess(List<int> roleModuleAccessIds)
        {
            var roleModuleAccessList = await _roleModuleAccessRepository.Delete(roleModuleAccessIds);

            //if (roleModuleAccessList == null)
            //    return NotFound("Error encountered when deleting roleModuleAccess!");

            return Ok(roleModuleAccessList);
        }
        #endregion
    }
}
