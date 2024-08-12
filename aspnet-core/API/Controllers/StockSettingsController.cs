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
    public class StockSettingsController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IStockSettingsRepository _stockSettingsRepository;

        public StockSettingsController(DataContext dataContext, IStockSettingsRepository stockSettingsRepository)
        {
            _dataContext = dataContext;
            _stockSettingsRepository = stockSettingsRepository;
        }

        #region Get Data
        [HttpGet("GetStockSettings")]
        public async Task<ActionResult<StockSettings>> GetStockSettings()
        {
            var result = await _stockSettingsRepository.GetStockSettings();
            if (result == null)
            {
                return BadRequest("Stock Settings not found!");
            }
            return Ok(result);
        }
        #endregion

        #region Save Data
        [HttpPut("UpdateStockSettings")]
        public async Task<ActionResult<bool>> UpdateStockSettings(StockSettings stockSettings)
        {
            return await _stockSettingsRepository.Update(stockSettings);
        }
        #endregion
    }
}
