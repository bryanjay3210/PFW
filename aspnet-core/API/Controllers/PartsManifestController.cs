using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO.Paginated;
using Domain.DomainModel.Interface;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class PartsManifestController : ControllerBase
    {
        private readonly IPartsManifestRepository _partsManifestRepository;
        
        public PartsManifestController(IPartsManifestRepository partsManifestRepository)
        {
            _partsManifestRepository = partsManifestRepository;
        }

        #region Get Data
        [HttpGet("GetPartsManifests")] 
        public async Task<ActionResult<List<PartsManifest>>> GetPartsManifests()
        {
            return Ok(await _partsManifestRepository.GetPartsManifests());
        }

        [HttpGet("GetPartsManifestById")]
        public async Task<ActionResult<PartsManifest>> GetPartsManifestById(int partsManifestId)
        {
            var partsManifest = await _partsManifestRepository.GetPartsManifest(partsManifestId);
            if (partsManifest == null)
                return NotFound("PartsManifest not found!");
            return Ok(partsManifest);
        }

        [HttpGet("GetPartsManifestsPaginated")]
        public async Task<ActionResult<PaginatedListDTO<PartsManifest>>> GetPartsManifestsPaginated(
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "PartsManifestNumber",
            [FromQuery] string? sortOrder = "DESC",
        [FromQuery] string? search = ""
            )
        {
            var result = await _partsManifestRepository.GetPartsManifestsPaginated(pageSize, pageIndex, sortColumn, sortOrder, search);
            return Ok(result);
        }

        [HttpGet("GetPartsManifestsByDatePaginated")]
        public async Task<ActionResult<PaginatedListDTO<PartsManifest>>> GetPartsManifestsByDatePaginated( int pageSize, int pageIndex, DateTime fromDate, DateTime toDate)
        {
            var result = await _partsManifestRepository.GetPartsManifestsByDatePaginated(pageSize, pageIndex, fromDate, toDate);
            return Ok(result);
        }
        #endregion

        #region Save Data
        [HttpPost("CreatePartsManifest")]
        public async Task<ActionResult<bool>> CreatePartsManifest(PartsManifest partsManifest)
        {
            var result = await _partsManifestRepository.Create(partsManifest);
            return Ok(result);
        }

        [HttpPut("UpdatePartsManifest")]
        public async Task<ActionResult<List<PartsManifest>>> UpdatePartsManifest(PartsManifest partsManifest)
        {
            var partsManifestList = await _partsManifestRepository.Update(partsManifest);
            
            //if (partsManifestList == null)
            //    return NotFound("Error encountered while updating partsManifest!");

            return Ok(partsManifestList);
        }

        [HttpDelete("DeletePartsManifest")]
        public async Task<ActionResult<List<PartsManifest>>> DeletePartsManifest(List<int> partsManifestIds)
        {
            var partsManifestList = await _partsManifestRepository.Delete(partsManifestIds);
            
            //if (partsManifestList == null)
            //    return NotFound("Error encountered when deleting partsManifest!");

            return Ok(partsManifestList);
        }
        #endregion
    }
}
