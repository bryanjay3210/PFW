using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Infrastucture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class DriverLogDetailController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IDriverLogDetailRepository _driverLogDetailRepository;

        public DriverLogDetailController(DataContext context, IDriverLogDetailRepository driverLogDetailRepository)
        {
            _context = context;
            _driverLogDetailRepository = driverLogDetailRepository;
        }

        #region Get Data
        [HttpGet("GetDriverLogDetails")] 
        public async Task<ActionResult<List<DriverLogDetail>>> GetDriverLogDetails()
        {
            return Ok(await _driverLogDetailRepository.GetDriverLogDetails());
        }

        [HttpGet("GetDriverLogDetailById")]
        public async Task<ActionResult<DriverLogDetail>> GetDriverLogDetailById(int driverLogDetailId)
        {
            var driverLogDetail = await _driverLogDetailRepository.GetDriverLogDetail(driverLogDetailId);
            if (driverLogDetail == null)
                return NotFound("DriverLogDetail not found!");
            return Ok(driverLogDetail);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateDriverLogDetail")]
        public async Task<ActionResult<List<DriverLogDetail>>> CreateDriverLogDetail(DriverLogDetail driverLogDetail)
        {
            var driverLogDetailList = await _driverLogDetailRepository.Create(driverLogDetail);

            //if (driverLogDetailList == null)
            //    return NotFound("New driverLogDetail not created!");
            
            return Ok(driverLogDetailList);
        }

        [HttpPut("UpdateDriverLogDetail")]
        public async Task<ActionResult<List<DriverLogDetail>>> UpdateDriverLogDetail(DriverLogDetail driverLogDetail)
        {
            var driverLogDetailList = await _driverLogDetailRepository.Update(driverLogDetail);
            
            //if (driverLogDetailList == null)
            //    return NotFound("Error encountered while updating driverLogDetail!");

            return Ok(driverLogDetailList);
        }

        [HttpDelete("DeleteDriverLogDetail")]
        public async Task<ActionResult<List<DriverLogDetail>>> DeleteDriverLogDetail(List<int> driverLogDetailIds)
        {
            var driverLogDetailList = await _driverLogDetailRepository.Delete(driverLogDetailIds);
            
            //if (driverLogDetailList == null)
            //    return NotFound("Error encountered when deleting driverLogDetail!");

            return Ok(driverLogDetailList);
        }
        #endregion
    }
}
