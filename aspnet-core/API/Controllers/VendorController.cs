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
    public class VendorController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IVendorRepository _vendorRepository;

        public VendorController(DataContext dataContext, IVendorRepository vendorRepository)
        {
            _dataContext = dataContext;
            _vendorRepository = vendorRepository;
        }

        #region Get Data
        [HttpGet("GetVendors")]
        public async Task<ActionResult<List<Vendor>>> GetVendors()
        {
            return Ok(await _vendorRepository.GetVendors());
        }

        [HttpGet("GetVendorsByState")]
        public async Task<ActionResult<List<Vendor>>> GetVendorsByState(string state)
        {
            return Ok(await _vendorRepository.GetVendorsByState(state));
        }

        [HttpGet("GetVendorById")]
        public async Task<ActionResult<Vendor>> GetVendorById(int vendorId)
        {
            var vendor = await _vendorRepository.GetVendor(vendorId);
            if (vendor == null)
                return NotFound("Vendor not found!");
            return Ok(vendor);
        }

        [HttpGet("GetVendorOrdersByVendorCode")]
        public async Task<ActionResult<List<VendorOrderDTO>>> GetVendorOrdersByVendorCode(string vendorCode)
        {
            List<VendorOrderDTO> vendorOrderList = await _vendorRepository.GetVendorOrdersByVendorCode(vendorCode);
            if (vendorOrderList == null)
                return NotFound("Vendor not found!");
            return Ok(vendorOrderList);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateVendor")]
        public async Task<ActionResult<List<Vendor>>> CreateVendor(Vendor vendor)
        {
            var vendorList = await _vendorRepository.Create(vendor);
            return Ok(vendorList);
        }

        [HttpPut("UpdateVendor")]
        public async Task<ActionResult<List<Vendor>>> UpdateVendor(Vendor vendor)
        {
            var vendorList = await _vendorRepository.Update(vendor);
            return Ok(vendorList);
        }

        [HttpDelete("DeleteVendor")]
        public async Task<ActionResult<List<Vendor>>> DeleteVendor(List<int> vendorIds)
        {
            var vendorList = await _vendorRepository.Delete(vendorIds);
            return Ok(vendorList);
        }
        #endregion
    }
}
