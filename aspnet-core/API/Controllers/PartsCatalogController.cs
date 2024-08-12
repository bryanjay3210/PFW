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
    public class PartsCatalogController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IPartsCatalogRepository _partsCatalogRepository;

        public PartsCatalogController(DataContext dataContext, IPartsCatalogRepository partsCatalogRepository)
        {
            _dataContext = dataContext;
            _partsCatalogRepository = partsCatalogRepository;
        }

        #region Get Data
        [HttpGet("GetPartsCatalogs")]
        public async Task<ActionResult<List<PartsCatalog>>> GetPartsCatalogs()
        {
            return Ok(await _partsCatalogRepository.GetPartsCatalogs());
        }

        [HttpGet("GetPartsCatalogsByProductId")]
        public async Task<ActionResult<List<PartsCatalog>>> GetPartsCatalogsByProductId(int productId)
        {
            return Ok(await _partsCatalogRepository.GetPartsCatalogsByProductId(productId));
        }

        [HttpGet("GetPartsCatalogById")]
        public async Task<ActionResult<PartsCatalog>> GetPartsCatalogById(int partsCatalogId)
        {
            var partsCatalog = await _partsCatalogRepository.GetPartsCatalog(partsCatalogId);
            if (partsCatalog == null)
                return NotFound("PartsCatalog not found!");
            return Ok(partsCatalog);
        }
        #endregion

        #region Save Data
        [HttpPost("CreatePartsCatalog")]
        public async Task<ActionResult<List<PartsCatalog>>> CreatePartsCatalog(PartsCatalog partsCatalog)
        {
            var partsCatalogList = await _partsCatalogRepository.Create(partsCatalog);
            return Ok(partsCatalogList);
        }

        [HttpPut("UpdatePartsCatalog")]
        public async Task<ActionResult<List<PartsCatalog>>> UpdatePartsCatalog(PartsCatalog partsCatalog)
        {
            var partsCatalogList = await _partsCatalogRepository.Update(partsCatalog);
            return Ok(partsCatalogList);
        }

        [HttpDelete("DeletePartsCatalog")]
        public async Task<ActionResult<List<PartsCatalog>>> DeletePartsCatalog(List<int> partsCatalogIds)
        {
            var partsCatalogList = await _partsCatalogRepository.Delete(partsCatalogIds);
            return Ok(partsCatalogList);
        }
        #endregion
    }
}
