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
    public class VendorCatalogController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IVendorCatalogRepository _vendorCatalogRepository;

        public VendorCatalogController(DataContext dataContext, IVendorCatalogRepository vendorCatalogRepository)
        {
            _dataContext = dataContext;
            _vendorCatalogRepository = vendorCatalogRepository;
        }

        #region Get Data
        [HttpGet("GetVendorCatalogs")]
        public async Task<ActionResult<List<VendorCatalog>>> GetVendorCatalogs()
        {
            return Ok(await _vendorCatalogRepository.GetVendorCatalogs());
        }

        [HttpGet("GetVendorCatalogById")]
        public async Task<ActionResult<VendorCatalog>> GetVendorCatalogById(int vendorCatalogId)
        {
            var vendorCatalog = await _vendorCatalogRepository.GetVendorCatalog(vendorCatalogId);
            if (vendorCatalog == null)
                return NotFound("Vendor Catalog not found!");
            return Ok(vendorCatalog);
        }

        [HttpPut("GetVendorCatalogsByPartsLinkNumbers")]
        public async Task<ActionResult<List<VendorCatalog>>> GetVendorCatalogsByPartsLinkNumbers(List<string> partsLinkNumbers)
        {
            return Ok(await _vendorCatalogRepository.GetVendorCatalogsByPartsLinkNumbers(partsLinkNumbers));
        }
        #endregion

        #region Save Data
        [HttpPost("CreateVendorCatalog")]
        public async Task<ActionResult<List<VendorCatalog>>> CreateVendorCatalog(VendorCatalog vendorCatalog)
        {
            var vendorCatalogList = await _vendorCatalogRepository.Create(vendorCatalog);
            return Ok(vendorCatalogList);
        }

        [HttpPut("UpdateVendorCatalog")]
        public async Task<ActionResult<List<VendorCatalog>>> UpdateVendorCatalog(VendorCatalog vendorCatalog)
        {
            var vendorCatalogList = await _vendorCatalogRepository.Update(vendorCatalog);
            return Ok(vendorCatalogList);
        }

        [HttpDelete("DeleteVendorCatalog")]
        public async Task<ActionResult<List<VendorCatalog>>> DeleteVendorCatalog(List<int> vendorCatalogIds)
        {
            var vendorCatalogList = await _vendorCatalogRepository.Delete(vendorCatalogIds);
            return Ok(vendorCatalogList);
        }

        [HttpPost("CreateVendorCatalogByProduct")]
        public async Task<ActionResult<List<VendorCatalog>>> CreateVendorCatalogByProduct(ProductVendorCatalog productVendorCatalog)
        {
            var vendorCatalogList = await _vendorCatalogRepository.CreateByProduct(productVendorCatalog.VendorCatalog, productVendorCatalog.PartsLinkNumbers);
            return Ok(vendorCatalogList);
        }

        [HttpPut("UpdateVendorCatalogByProduct")]
        public async Task<ActionResult<List<VendorCatalog>>> UpdateVendorCatalogByProduct(ProductVendorCatalog productVendorCatalog)
        {
            var vendorCatalogList = await _vendorCatalogRepository.UpdateByProduct(productVendorCatalog.VendorCatalog, productVendorCatalog.PartsLinkNumbers);
            return Ok(vendorCatalogList);
        }
        #endregion
    }
}
