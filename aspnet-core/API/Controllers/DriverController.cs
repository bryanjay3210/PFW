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
    public class DriverController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IDriverRepository _driverRepository;

        public DriverController(DataContext context, IDriverRepository driverRepository)
        {
            _context = context;
            _driverRepository = driverRepository;
        }

        #region Get Data
        [HttpGet("GetDrivers")] 
        public async Task<ActionResult<List<Driver>>> GetDrivers()
        {
            return Ok(await _driverRepository.GetDrivers());
        }

        [HttpGet("GetDriverById")]
        public async Task<ActionResult<Driver>> GetDriverById(int driverId)
        {
            var driver = await _driverRepository.GetDriver(driverId);
            if (driver == null)
                return NotFound("Driver not found!");
            return Ok(driver);
        }

        [HttpGet("GetDriverByDriverNumber")]
        public async Task<ActionResult<Driver>> GetDriverByDriverNumber(string driverNumber)
        {
            var driver = await _driverRepository.GetDriverByDriverNumber(driverNumber);
            if (driver == null)
                return NotFound("Driver not found!");
            return Ok(driver);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateDriver")]
        public async Task<ActionResult<List<Driver>>> CreateDriver(Driver driver)
        {
            var driverList = await _driverRepository.Create(driver);

            //if (driverList == null)
            //    return NotFound("New driver not created!");
            
            return Ok(driverList);
        }

        [HttpPut("UpdateDriver")]
        public async Task<ActionResult<List<Driver>>> UpdateDriver(Driver driver)
        {
            var driverList = await _driverRepository.Update(driver);
            
            //if (driverList == null)
            //    return NotFound("Error encountered while updating driver!");

            return Ok(driverList);
        }

        [HttpDelete("DeleteDriver")]
        public async Task<ActionResult<List<Driver>>> DeleteDriver(List<int> driverIds)
        {
            var driverList = await _driverRepository.Delete(driverIds);
            
            //if (driverList == null)
            //    return NotFound("Error encountered when deleting driver!");

            return Ok(driverList);
        }
        #endregion
    }
}
