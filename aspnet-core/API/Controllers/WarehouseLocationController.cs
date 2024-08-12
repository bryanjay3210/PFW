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
    public class WarehouseLocationController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IWarehouseLocationRepository _warehouseLocationRepository;

        public WarehouseLocationController(DataContext dataContext, IWarehouseLocationRepository warehouseLocationRepository)
        {
            _dataContext = dataContext;
            _warehouseLocationRepository = warehouseLocationRepository;
        }

        #region Get Data
        [HttpGet("GetWarehouseLocations")]
        public async Task<ActionResult<List<WarehouseLocation>>> GetWarehouseLocations()
        {
            return Ok(await _warehouseLocationRepository.GetWarehouseLocations());
        }

        [HttpGet("GetWarehouseLocationById")]
        public async Task<ActionResult<WarehouseLocation>> GetWarehouseLocationById(int warehouseLocationId)
        {
            var warehouseLocation = await _warehouseLocationRepository.GetWarehouseLocation(warehouseLocationId);
            if (warehouseLocation == null)
                return NotFound("WarehouseLocation not found!");
            return Ok(warehouseLocation);
        }

        [HttpGet("GetWarehouseLocationByLocation")]
        public async Task<ActionResult<WarehouseLocation>> GetWarehouseLocationByLocation(int state, string location)
        {
            var warehouseLocation = await _warehouseLocationRepository.GetWarehouseLocationByLocation(state, location);
            //if (warehouseLocation == null)
            //    return NotFound("WarehouseLocation not found!");
            return Ok(warehouseLocation);
        }

        [HttpGet("GetWarehouseLocationByLocationWithStocks")]
        public async Task<ActionResult<WarehouseLocationWithStockDTO>> GetWarehouseLocationByLocationWithStocks(int state, string location)
        {
            var warehouseLocationWithstock = await _warehouseLocationRepository.GetWarehouseLocationByLocationWithStocks(state, location);
            return Ok(warehouseLocationWithstock);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateWarehouseLocation")]
        public async Task<ActionResult<List<WarehouseLocation>>> CreateWarehouseLocation(WarehouseLocation warehouseLocation)
        {
            var warehouseLocationList = await _warehouseLocationRepository.Create(warehouseLocation);
            return Ok(warehouseLocationList);
        }

        [HttpPost("CreateWarehouseLocationWithStock")]
        public async Task<ActionResult<List<WarehousePartDTO>>> CreateWarehouseLocationWithStock(WarehouseLocation warehouseLocation)
        {
            var warehousePartList = await _warehouseLocationRepository.CreateWithStock(warehouseLocation);
            return Ok(warehousePartList);
        }

        [HttpPut("UpdateWarehouseLocation")]
        public async Task<ActionResult<List<WarehouseLocation>>> UpdateWarehouseLocation(WarehouseLocation warehouseLocation)
        {
            var warehouseLocationList = await _warehouseLocationRepository.Update(warehouseLocation);
            return Ok(warehouseLocationList);
        }

        [HttpPut("UpdateWarehouseLocationWithStock")]
        public async Task<ActionResult<List<WarehousePartDTO>>> UpdateWarehouseLocationWithStock(WarehouseLocation warehouseLocation)
        {
            var warehousePartList = await _warehouseLocationRepository.UpdateWithStock(warehouseLocation);
            return Ok(warehousePartList);
        }

        [HttpDelete("DeleteWarehouseLocation")]
        public async Task<ActionResult<List<WarehouseLocation>>> DeleteWarehouseLocation(List<int> warehouseLocationIds)
        {
            var warehouseLocationList = await _warehouseLocationRepository.Delete(warehouseLocationIds);
            return Ok(warehouseLocationList);
        }
        #endregion
    }
}
