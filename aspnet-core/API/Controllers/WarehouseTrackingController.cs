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
    public class WarehouseController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IWarehouseRepository _warehouseRepository;

        public WarehouseController(DataContext dataContext, IWarehouseRepository warehouseRepository)
        {
            _dataContext = dataContext;
            _warehouseRepository = warehouseRepository;
        }

        #region Get Data
        [HttpGet("GetWarehouses")]
        public async Task<ActionResult<List<Warehouse>>> GetWarehouses()
        {
            return Ok(await _warehouseRepository.GetWarehouses());
        }

        [HttpGet("GetWarehouseById")]
        public async Task<ActionResult<Warehouse>> GetWarehouseById(int warehouseId)
        {
            var warehouse = await _warehouseRepository.GetWarehouse(warehouseId);
            if (warehouse == null)
                return NotFound("Warehouse not found!");
            return Ok(warehouse);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateWarehouse")]
        public async Task<ActionResult<List<Warehouse>>> CreateWarehouse(Warehouse warehouse)
        {
            var warehouseList = await _warehouseRepository.Create(warehouse);

            //if (warehouseList == null)
            //    return NotFound("New warehouse not created!");

            return Ok(warehouseList);
        }

        [HttpPut("UpdateWarehouse")]
        public async Task<ActionResult<List<Warehouse>>> UpdateWarehouse(Warehouse warehouse)
        {
            var warehouseList = await _warehouseRepository.Update(warehouse);

            //if (warehouseList == null)
            //    return NotFound("Error encountered while updating warehouse!");

            return Ok(warehouseList);
        }

        [HttpDelete("DeleteWarehouse")]
        public async Task<ActionResult<List<Warehouse>>> DeleteWarehouse(List<int> warehouseIds)
        {
            var warehouseList = await _warehouseRepository.Delete(warehouseIds);

            //if (warehouseList == null)
            //    return NotFound("Error encountered when deleting warehouse!");

            return Ok(warehouseList);
        }
        #endregion
    }
}
