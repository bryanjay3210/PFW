using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Infrastucture;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class PartsManifestDetailController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IPartsManifestDetailRepository _partsManifestDetailRepository;

        public PartsManifestDetailController(DataContext context, IPartsManifestDetailRepository partsManifestDetailRepository)
        {
            _context = context;
            _partsManifestDetailRepository = partsManifestDetailRepository;
        }

        #region Get Data
        [HttpGet("GetPartsManifestDetails")] 
        public async Task<ActionResult<List<PartsManifestDetail>>> GetPartsManifestDetails()
        {
            return Ok(await _partsManifestDetailRepository.GetPartsManifestDetails());
        }

        [HttpGet("GetPartsManifestDetailById")]
        public async Task<ActionResult<PartsManifestDetail>> GetPartsManifestDetailById(int partsManifestDetailId)
        {
            var partsManifestDetail = await _partsManifestDetailRepository.GetPartsManifestDetail(partsManifestDetailId);
            if (partsManifestDetail == null)
                return NotFound("PartsManifestDetail not found!");
            return Ok(partsManifestDetail);
        }
        #endregion

        #region Save Data
        [HttpPost("CreatePartsManifestDetail")]
        public async Task<ActionResult<List<PartsManifestDetail>>> CreatePartsManifestDetail(PartsManifestDetail partsManifestDetail)
        {
            var partsManifestDetailList = await _partsManifestDetailRepository.Create(partsManifestDetail);

            //if (partsManifestDetailList == null)
            //    return NotFound("New partsManifestDetail not created!");
            
            return Ok(partsManifestDetailList);
        }

        [HttpPut("UpdatePartsManifestDetail")]
        public async Task<ActionResult<List<PartsManifestDetail>>> UpdatePartsManifestDetail(PartsManifestDetail partsManifestDetail)
        {
            var partsManifestDetailList = await _partsManifestDetailRepository.Update(partsManifestDetail);
            
            //if (partsManifestDetailList == null)
            //    return NotFound("Error encountered while updating partsManifestDetail!");

            return Ok(partsManifestDetailList);
        }

        [HttpDelete("DeletePartsManifestDetail")]
        public async Task<ActionResult<List<PartsManifestDetail>>> DeletePartsManifestDetail(List<int> partsManifestDetailIds)
        {
            var partsManifestDetailList = await _partsManifestDetailRepository.Delete(partsManifestDetailIds);
            
            //if (partsManifestDetailList == null)
            //    return NotFound("Error encountered when deleting partsManifestDetail!");

            return Ok(partsManifestDetailList);
        }
        #endregion
    }
}
