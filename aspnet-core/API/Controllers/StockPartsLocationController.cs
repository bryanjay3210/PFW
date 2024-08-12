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
    public class StockPartsLocationController : ControllerBase
    {
        private readonly IStockPartsLocationRepository _stockPartsLocationRepository;

        public StockPartsLocationController(IStockPartsLocationRepository stockPartsLocationRepository)
        {
            _stockPartsLocationRepository = stockPartsLocationRepository ?? throw new ArgumentNullException(nameof(stockPartsLocationRepository));
        }

        #region Get Data
        [HttpGet("GetStockPartsLocations")]
        public async Task<ActionResult<List<StockPartsLocation>>> GetStockPartsLocations()
        {
            try
            {
                return Ok(await _stockPartsLocationRepository.GetStockPartsLocations());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("GetStockPartsLocationsByPartNumber")]
        public async Task<ActionResult<List<StockPartsLocation>>> GetStockPartsLocationsByPartNumber(string partNumber)
        {
            try
            {
                return Ok(await _stockPartsLocationRepository.GetStockPartsLocationsByPartNumber(partNumber));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("GetStockPartsLocationById")]
        public async Task<ActionResult<StockPartsLocation>> GetStockPartsLocationById(int stockPartsLocationId)
        {
            try
            {
                var stockPartsLocation = await _stockPartsLocationRepository.GetStockPartsLocation(stockPartsLocationId);
                if (stockPartsLocation == null)
                    return NotFound("StockPartsLocation not found!");
                return Ok(stockPartsLocation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        #endregion

        #region Save Data
        [HttpPost("CreateStockPartsLocation")]
        public async Task<ActionResult<List<StockPartsLocation>>> CreateStockPartsLocation(StockPartsLocation stockPartsLocation)
        {
            try
            {
                var stockPartsLocationList = await _stockPartsLocationRepository.Create(stockPartsLocation);
                if (stockPartsLocationList == null || stockPartsLocationList.Count == 0)
                    return BadRequest("An error was encountered while creating the Stock Parts Location.");

                return Ok(stockPartsLocationList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateStockPartsLocation")]
        public async Task<ActionResult<List<StockPartsLocation>>> UpdateStockPartsLocation(StockPartsLocation stockPartsLocation)
        {
            try 
            {
                var stockPartsLocationList = await _stockPartsLocationRepository.Update(stockPartsLocation);
                if (stockPartsLocationList == null || stockPartsLocationList.Count == 0)
                    return BadRequest("An error was encountered while updating the Stock Parts Location.");

                return Ok(stockPartsLocationList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteStockPartsLocation")]
        public async Task<ActionResult<List<StockPartsLocation>>> DeleteStockPartsLocation(List<int> stockPartsLocationIds)
        {
            try
            {
                var stockPartsLocationList = await _stockPartsLocationRepository.Delete(stockPartsLocationIds);
                if (stockPartsLocationList == null)
                    return BadRequest("An error was encountered while deleting the Stock Parts Location.");

                return Ok(stockPartsLocationList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
