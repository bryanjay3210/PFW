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
    public class RolePermissionController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IRolePermissionRepository _rolePermissionRepository;

        public RolePermissionController(DataContext dataContext, IRolePermissionRepository rolePermissionRepository)
        {
            _dataContext = dataContext;
            _rolePermissionRepository = rolePermissionRepository;
        }

        #region Get Data
        [HttpGet("GetRolePermissions")]
        public async Task<ActionResult<List<RolePermission>>> GetRolePermissions()
        {
            return Ok(await _rolePermissionRepository.GetRolePermissions());
        }

        [HttpGet("GetRolePermissionById")]
        public async Task<ActionResult<RolePermission>> GetRolePermissionById(int rolePermissionId)
        {
            var rolePermission = await _rolePermissionRepository.GetRolePermission(rolePermissionId);
            if (rolePermission == null)
                return NotFound("RolePermission not found!");
            return Ok(rolePermission);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateRolePermission")]
        public async Task<ActionResult<List<RolePermission>>> CreateRolePermission(RolePermission rolePermission)
        {
            var rolePermissionList = await _rolePermissionRepository.Create(rolePermission);

            //if (rolePermissionList == null)
            //    return NotFound("New rolePermission not created!");

            return Ok(rolePermissionList);
        }

        [HttpPut("UpdateRolePermission")]
        public async Task<ActionResult<List<RolePermission>>> UpdateRolePermission(RolePermission rolePermission)
        {
            var rolePermissionList = await _rolePermissionRepository.Update(rolePermission);

            //if (rolePermissionList == null)
            //    return NotFound("Error encountered while updating rolePermission!");

            return Ok(rolePermissionList);
        }

        [HttpDelete("DeleteRolePermission")]
        public async Task<ActionResult<List<RolePermission>>> DeleteRolePermission(List<int> rolePermissionIds)
        {
            var rolePermissionList = await _rolePermissionRepository.Delete(rolePermissionIds);

            //if (rolePermissionList == null)
            //    return NotFound("Error encountered when deleting rolePermission!");

            return Ok(rolePermissionList);
        }
        #endregion
    }
}
