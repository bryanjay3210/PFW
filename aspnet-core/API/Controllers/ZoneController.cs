using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Infrastucture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ZoneController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IZoneRepository _zoneRepository;

        public ZoneController(DataContext dataContext, IZoneRepository zoneRepository)
        {
            _dataContext = dataContext;
            _zoneRepository = zoneRepository;
        }

        #region Get Data
        [HttpGet("GetZones")]
        public async Task<ActionResult<List<Zone>>> GetZones()
        {
            return Ok(await _zoneRepository.GetZones());
        }

        [HttpGet("GetZoneById")]
        public async Task<ActionResult<Zone>> GetZoneById(int zoneId)
        {
            var zone = await _zoneRepository.GetZone(zoneId);
            if (zone == null)
                return NotFound("Zone not found!");
            return Ok(zone);
        }

        [HttpGet("GetZoneByZipCode")]
        public async Task<ActionResult<Zone>> GetZoneByZipCode(string zipCode)
        {
            var zone = await _zoneRepository.GetZoneByZipCode(zipCode);
            if (zone == null)
                return NotFound("Zone not found!");
            return Ok(zone);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateZone")]
        public async Task<ActionResult<List<Zone>>> CreateZone(Zone zone)
        {
            var zoneList = await _zoneRepository.Create(zone);

            //if (zoneList == null)
            //    return NotFound("New zone not created!");

            return Ok(zoneList);
        }

        [HttpPut("UpdateZone")]
        public async Task<ActionResult<List<Zone>>> UpdateZone(Zone zone)
        {
            var zoneList = await _zoneRepository.Update(zone);

            //if (zoneList == null)
            //    return NotFound("Error encountered while updating zone!");

            return Ok(zoneList);
        }

        [HttpDelete("DeleteZone")]
        public async Task<ActionResult<List<Zone>>> DeleteZone(List<int> zoneIds)
        {
            var zoneList = await _zoneRepository.Delete(zoneIds);

            //if (zoneList == null)
            //    return NotFound("Error encountered when deleting zone!");

            return Ok(zoneList);
        }
        #endregion
    }
}
