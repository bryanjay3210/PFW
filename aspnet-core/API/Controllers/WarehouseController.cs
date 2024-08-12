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
    public class WarehouseTrackingController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IWarehouseTrackingRepository _warehouseTrackingRepository;

        public WarehouseTrackingController(DataContext dataContext, IWarehouseTrackingRepository warehouseTrackingRepository)
        {
            _dataContext = dataContext;
            _warehouseTrackingRepository = warehouseTrackingRepository;
        }

        #region Get Data
        [HttpGet("GetWarehouseTrackings")]
        public async Task<ActionResult<List<WarehouseTracking>>> GetWarehouseTrackings()
        {
            return Ok(await _warehouseTrackingRepository.GetWarehouseTrackings());
        }

        [HttpGet("GetWarehouseTrackingById")]
        public async Task<ActionResult<WarehouseTracking>> GetWarehouseTrackingById(int warehouseTrackingId)
        {
            var warehouseTracking = await _warehouseTrackingRepository.GetWarehouseTracking(warehouseTrackingId);
            if (warehouseTracking == null)
                return NotFound("Warehouse Tracking not found!");
            return Ok(warehouseTracking);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateWarehouseTracking")]
        public async Task<ActionResult<List<WarehouseTracking>>> CreateWarehouseTracking(WarehouseTracking warehouseTracking)
        {
            var warehouseTrackingList = await _warehouseTrackingRepository.Create(warehouseTracking);

            //if (warehouseTrackingList == null)
            //    return NotFound("New warehouseTracking not created!");

            return Ok(warehouseTrackingList);
        }

        [HttpPut("UpdateWarehouseTracking")]
        public async Task<ActionResult<List<WarehouseTracking>>> UpdateWarehouseTracking(WarehouseTracking warehouseTracking)
        {
            var warehouseTrackingList = await _warehouseTrackingRepository.Update(warehouseTracking);

            //if (warehouseTrackingList == null)
            //    return NotFound("Error encountered while updating warehouseTracking!");

            return Ok(warehouseTrackingList);
        }

        [HttpDelete("DeleteWarehouseTracking")]
        public async Task<ActionResult<List<WarehouseTracking>>> DeleteWarehouseTracking(List<int> warehouseTrackingIds)
        {
            var warehouseTrackingList = await _warehouseTrackingRepository.Delete(warehouseTrackingIds);

            //if (warehouseTrackingList == null)
            //    return NotFound("Error encountered when deleting warehouseTracking!");

            return Ok(warehouseTrackingList);
        }
        #endregion
    }
}
