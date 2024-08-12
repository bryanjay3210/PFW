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
    public class ModuleGroupController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IModuleGroupRepository _moduleGroupRepository;

        public ModuleGroupController(DataContext dataContext, IModuleGroupRepository moduleGroupRepository)
        {
            _dataContext = dataContext;
            _moduleGroupRepository = moduleGroupRepository;
        }

        #region Get Data
        [HttpGet("GetModuleGroups")]
        public async Task<ActionResult<List<ModuleGroup>>> GetModuleGroups()
        {
            return Ok(await _moduleGroupRepository.GetModuleGroups());
        }

        [HttpGet("GetModuleGroupById")]
        public async Task<ActionResult<ModuleGroup>> GetModuleGroupById(int moduleGroupId)
        {
            var moduleGroup = await _moduleGroupRepository.GetModuleGroup(moduleGroupId);
            if (moduleGroup == null)
                return NotFound("ModuleGroup not found!");
            return Ok(moduleGroup);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateModuleGroup")]
        public async Task<ActionResult<List<ModuleGroup>>> CreateModuleGroup(ModuleGroup moduleGroup)
        {
            var moduleGroupList = await _moduleGroupRepository.Create(moduleGroup);

            //if (moduleGroupList == null)
            //    return NotFound("New moduleGroup not created!");

            return Ok(moduleGroupList);
        }

        [HttpPut("UpdateModuleGroup")]
        public async Task<ActionResult<List<ModuleGroup>>> UpdateModuleGroup(ModuleGroup moduleGroup)
        {
            var moduleGroupList = await _moduleGroupRepository.Update(moduleGroup);

            //if (moduleGroupList == null)
            //    return NotFound("Error encountered while updating moduleGroup!");

            return Ok(moduleGroupList);
        }

        [HttpDelete("DeleteModuleGroup")]
        public async Task<ActionResult<List<ModuleGroup>>> DeleteModuleGroup(List<int> moduleGroupIds)
        {
            var moduleGroupList = await _moduleGroupRepository.Delete(moduleGroupIds);

            //if (moduleGroupList == null)
            //    return NotFound("Error encountered when deleting moduleGroup!");

            return Ok(moduleGroupList);
        }
        #endregion
    }
}
