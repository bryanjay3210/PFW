using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Interface;
using Infrastucture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly ILocationRepository _locationRepository;

        public LocationController(DataContext dataContext, ILocationRepository locationRepository)
        {
            _dataContext = dataContext;
            _locationRepository = locationRepository;
        }

        #region Get Data
        [HttpGet("GetLocations")]
        public async Task<ActionResult<List<Location>>> GetLocations()
        {
            return Ok(await _locationRepository.GetLocations());
        }

        [HttpGet("GetLocationsByCustomerId")]
        public async Task<ActionResult<List<Location>>> GetLocationsByCustomerId(int customerId)
        {
            return Ok(await _locationRepository.GetLocationsByCustomerId(customerId));
        }

        [HttpGet("GetLocationsList")]
        public async Task<ActionResult<List<LocationDTO>>> GetLocationsList(int customerId)
        {
            return Ok(await _locationRepository.GetLocationsList(customerId));
        }

        [HttpGet("GetLocationById")]
        public async Task<ActionResult<Location>> GetLocationById(int locationId)
        {
            var location = await _locationRepository.GetLocation(locationId);
            if (location == null)
                return NotFound("Location not found!");
            return Ok(location);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateLocation")]
        public async Task<ActionResult<List<Location>>> CreateLocation(Location location)
        {
            var locationList = await _locationRepository.Create(location);

            //if (locationList == null)
            //    return NotFound("New location not created!");

            return Ok(locationList);
        }

        [HttpPut("UpdateLocation")]
        public async Task<ActionResult<List<Location>>> UpdateLocation(Location location)
        {
            var locationList = await _locationRepository.Update(location);

            //if (locationList == null)
            //    return NotFound("Error encountered while updating location!");

            return Ok(locationList);
        }

        [HttpDelete("DeleteLocation")]
        public async Task<ActionResult<List<Location>>> DeleteLocation(List<int> locationIds)
        {
            var locationList = await _locationRepository.Delete(locationIds);

            //if (locationList == null)
            //    return NotFound("Error encountered when deleting location!");

            return Ok(locationList);
        }
        #endregion
    }
}
