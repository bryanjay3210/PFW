using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Infrastucture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AutomobileController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IAutomobileRepository _automobileRepository;

        public AutomobileController(DataContext context, IAutomobileRepository automobileRepository)
        {
            _context = context;
            _automobileRepository = automobileRepository;
        }

        #region Get Data
        [HttpGet("GetAutomobiles")] 
        public async Task<ActionResult<List<Automobile>>> GetAutomobiles()
        {
            return Ok(await _automobileRepository.GetAutomobiles());
        }

        [HttpGet("GetAutomobileById")]
        public async Task<ActionResult<Automobile>> GetAutomobileById(int automobileId)
        {
            var automobile = await _automobileRepository.GetAutomobile(automobileId);
            if (automobile == null)
                return NotFound("Automobile not found!");
            return Ok(automobile);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateAutomobile")]
        public async Task<ActionResult<List<Automobile>>> CreateAutomobile(Automobile automobile)
        {
            var automobileList = await _automobileRepository.Create(automobile);

            //if (automobileList == null)
            //    return NotFound("New automobile not created!");
            
            return Ok(automobileList);
        }

        [HttpPut("UpdateAutomobile")]
        public async Task<ActionResult<List<Automobile>>> UpdateAutomobile(Automobile automobile)
        {
            var automobileList = await _automobileRepository.Update(automobile);
            
            //if (automobileList == null)
            //    return NotFound("Error encountered while updating automobile!");

            return Ok(automobileList);
        }

        [HttpDelete("DeleteAutomobile")]
        public async Task<ActionResult<List<Automobile>>> DeleteAutomobile(List<int> automobileIds)
        {
            var automobileList = await _automobileRepository.Delete(automobileIds);
            
            //if (automobileList == null)
            //    return NotFound("Error encountered when deleting automobile!");

            return Ok(automobileList);
        }
        #endregion
    }
}
