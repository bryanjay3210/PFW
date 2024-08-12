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
    public class ModuleController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IModuleRepository _moduleRepository;

        public ModuleController(DataContext dataContext, IModuleRepository moduleRepository)
        {
            _dataContext = dataContext;
            _moduleRepository = moduleRepository;
        }

        #region Get Data
        [HttpGet("GetModules")]
        public async Task<ActionResult<List<Module>>> GetModules()
        {
            return Ok(await _moduleRepository.GetModules());
        }

        [HttpGet("GetModuleById")]
        public async Task<ActionResult<Module>> GetModuleById(int moduleId)
        {
            var module = await _moduleRepository.GetModule(moduleId);
            if (module == null)
                return NotFound("Module not found!");
            return Ok(module);
        }

        [HttpGet("GetModulesByModuleGroup")]
        public async Task<ActionResult<List<Module>>> GetModulesByModuleGroup(int moduleGroupId)
        {
            var modules = await _moduleRepository.GetModulesByModuleGroup(moduleGroupId);
            if (modules.Count() == 0)
                return NotFound("Modules not found!");
            return Ok(modules);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateModule")]
        public async Task<ActionResult<List<Module>>> CreateModule(Module module)
        {
            var moduleList = await _moduleRepository.Create(module);

            //if (moduleList == null)
            //    return NotFound("New module not created!");

            return Ok(moduleList);
        }

        [HttpPut("UpdateModule")]
        public async Task<ActionResult<List<Module>>> UpdateModule(Module module)
        {
            var moduleList = await _moduleRepository.Update(module);

            //if (moduleList == null)
            //    return NotFound("Error encountered while updating module!");

            return Ok(moduleList);
        }

        [HttpDelete("DeleteModule")]
        public async Task<ActionResult<List<Module>>> DeleteModule(List<int> moduleIds)
        {
            var moduleList = await _moduleRepository.Delete(moduleIds);

            //if (moduleList == null)
            //    return NotFound("Error encountered when deleting module!");

            return Ok(moduleList);
        }
        #endregion
    }
}
