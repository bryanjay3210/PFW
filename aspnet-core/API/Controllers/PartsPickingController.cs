using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.DTO.Paginated;
using Domain.DomainModel.Interface;
using Infrastucture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PartsPickingController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IPartsPickingRepository _partsPickingRepository;

        public PartsPickingController(DataContext dataContext, IPartsPickingRepository partsPickingRepository)
        {
            _dataContext = dataContext;
            _partsPickingRepository = partsPickingRepository;
        }

        #region Get Data
        [HttpGet("GetPartsPickings")]
        public async Task<ActionResult<List<PartsPicking>>> GetPartsPickings()
        {
            return Ok(await _partsPickingRepository.GetPartsPickings());
        }

        [HttpGet("GetPartsPickingById")]
        public async Task<ActionResult<PartsPicking>> GetPartsPickingById(int partsPickingId)
        {
            var partsPicking = await _partsPickingRepository.GetPartsPicking(partsPickingId);
            if (partsPicking == null)
                return NotFound("PartsPicking not found!");
            return Ok(partsPicking);
        }

        [HttpGet("GetPartsPickingsPaginated")]
        public async Task<ActionResult<PaginatedListDTO<PartsPicking>>> GetPartsPickingsPaginated(
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "PickNumber",
            [FromQuery] string? sortOrder = "DESC",
            [FromQuery] string? search = ""
            )
        {
            var result = await _partsPickingRepository.GetPartsPickingsPaginated(pageSize, pageIndex, sortColumn, sortOrder, search);
            return Ok(result);
        }

        [HttpGet("GetStockOrderDetails")]
        public async Task<ActionResult<List<StockOrderDetailDTO>>> GetStockOrderDetails(int warehouseFilter)
        {
            var result = await _partsPickingRepository.GetStockOrderDetails(warehouseFilter);
            return Ok(result);
        }
        #endregion

        #region Save Data
        [HttpPost("CreatePartsPicking")]
        public async Task<ActionResult<bool>> CreatePartsPicking(PartsPicking partsPicking)
        {
            var result = await _partsPickingRepository.Create(partsPicking);
            return Ok(result);
        }

        [HttpPut("UpdatePartsPicking")]
        public async Task<ActionResult<bool>> UpdatePartsPicking(PartsPicking partsPicking)
        {
            var result = await _partsPickingRepository.Update(partsPicking);
            return Ok(result);
        }

        [HttpPut("SoftDeletePartsPickingDetail")]
        public async Task<ActionResult<bool>> SoftDeletePartsPickingDetail(PartsPickingDetail partsPickingDetail)
        {
            var result = await _partsPickingRepository.SoftDeletePartsPickingDetail(partsPickingDetail);
            return Ok(result);
        }

        [HttpDelete("DeletePartsPicking")]
        public async Task<ActionResult<bool>> DeletePartsPicking(PartsPicking partsPicking)
        {
            var partsPickingList = await _partsPickingRepository.Delete(partsPicking);
            return Ok(partsPickingList);
        }
        #endregion
    }
}
