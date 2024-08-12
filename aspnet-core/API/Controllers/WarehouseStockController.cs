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
    public class WarehouseStockController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IWarehouseStockRepository _warehouseStockRepository;

        public WarehouseStockController(DataContext dataContext, IWarehouseStockRepository warehouseStockRepository)
        {
            _dataContext = dataContext;
            _warehouseStockRepository = warehouseStockRepository;
        }

        #region Get Data
        [HttpGet("GetWarehouseStocks")]
        public async Task<ActionResult<List<WarehouseStock>>> GetWarehouseStocks()
        {
            return Ok(await _warehouseStockRepository.GetWarehouseStocks());
        }

        [HttpGet("GetWarehouseStockById")]
        public async Task<ActionResult<WarehouseStock>> GetWarehouseStockById(int warehouseStockId)
        {
            var warehouseStock = await _warehouseStockRepository.GetWarehouseStock(warehouseStockId);
            if (warehouseStock == null)
                return NotFound("WarehouseStock not found!");
            return Ok(warehouseStock);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateWarehouseStock")]
        public async Task<ActionResult<List<WarehouseStock>>> CreateWarehouseStock(WarehouseStock warehouseStock)
        {
            var warehouseStockList = await _warehouseStockRepository.Create(warehouseStock);

            //if (warehouseStockList == null)
            //    return NotFound("New warehouseStock not created!");

            return Ok(warehouseStockList);
        }

        [HttpPut("UpdateWarehouseStocks")]
        public async Task<ActionResult<bool>> UpdateWarehouseStocks([FromBody]List<WarehouseStockDTO> warehouseStocks)
        {
            var result = await _warehouseStockRepository.UpdateWarehouseStocks(warehouseStocks);
            return Ok(result);
        }

        [HttpPut("UpdateCycleCount")]
        public async Task<ActionResult<bool>> UpdateCycleCount([FromBody] List<WarehouseStockDTO> warehouseStocks)
        {
            var result = await _warehouseStockRepository.UpdateCycleCount(warehouseStocks);
            return Ok(result);
        }

        [HttpPut("TransferWarehouseStocks")]
        public async Task<ActionResult<bool>> TransferWarehouseStocks([FromBody] List<WarehouseStockDTO> warehouseStocks)
        {
            var result = await _warehouseStockRepository.TransferWarehouseStocks(warehouseStocks);
            return Ok(result);
        }

        [HttpPut("UpdateWarehouseStock")]
        public async Task<ActionResult<List<WarehouseStock>>> UpdateWarehouseStock(WarehouseStock warehouseStock)
        {
            var warehouseStockList = await _warehouseStockRepository.Update(warehouseStock);

            //if (warehouseStockList == null)
            //    return NotFound("Error encountered while updating warehouseStock!");

            return Ok(warehouseStockList);
        }

        [HttpDelete("DeleteWarehouseStock")]
        public async Task<ActionResult<List<WarehouseStock>>> DeleteWarehouseStock(List<int> warehouseStockIds)
        {
            var warehouseStockList = await _warehouseStockRepository.Delete(warehouseStockIds);

            //if (warehouseStockList == null)
            //    return NotFound("Error encountered when deleting warehouseStock!");

            return Ok(warehouseStockList);
        }
        #endregion
    }
}
