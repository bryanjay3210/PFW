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
    public class RoleController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IRoleRepository _roleRepository;

        public RoleController(DataContext dataContext, IRoleRepository roleRepository)
        {
            _dataContext = dataContext;
            _roleRepository = roleRepository;
        }

        #region Get Data
        [HttpGet("GetRoles")]
        public async Task<ActionResult<List<Role>>> GetRoles()
        {
            return Ok(await _roleRepository.GetRoles());
        }

        [HttpGet("GetRoleById")]
        public async Task<ActionResult<Role>> GetRoleById(int roleId)
        {
            var role = await _roleRepository.GetRole(roleId);
            if (role == null)
                return NotFound("Role not found!");
            return Ok(role);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateRole")]
        public async Task<ActionResult<List<Role>>> CreateRole(Role role)
        {
            var roleList = await _roleRepository.Create(role);

            //if (roleList == null)
            //    return NotFound("New role not created!");

            return Ok(roleList);
        }

        [HttpPut("UpdateRole")]
        public async Task<ActionResult<List<Role>>> UpdateRole(Role role)
        {
            var roleList = await _roleRepository.Update(role);

            //if (roleList == null)
            //    return NotFound("Error encountered while updating role!");

            return Ok(roleList);
        }

        [HttpDelete("DeleteRole")]
        public async Task<ActionResult<List<Role>>> DeleteRole(List<int> roleIds)
        {
            var roleList = await _roleRepository.Delete(roleIds);

            //if (roleList == null)
            //    return NotFound("Error encountered when deleting role!");

            return Ok(roleList);
        }
        #endregion
    }
}
