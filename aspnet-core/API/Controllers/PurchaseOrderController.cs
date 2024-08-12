using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.DTO.Paginated;
using Domain.DomainModel.Interface;
using Infrastucture;
using Infrastucture.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;

        public PurchaseOrderController(DataContext dataContext, IPurchaseOrderRepository purchaseOrderRepository)
        {
            _dataContext = dataContext;
            _purchaseOrderRepository = purchaseOrderRepository;
        }

        #region Get Data
        [HttpGet("GetPurchaseOrders")]
        public async Task<ActionResult<List<PurchaseOrder>>> GetPurchaseOrders()
        {
            return Ok(await _purchaseOrderRepository.GetPurchaseOrders());
        }

        [HttpGet("GetPurchaseOrderById")]
        public async Task<ActionResult<PurchaseOrder>> GetPurchaseOrderById(int purchaseOrderId)
        {
            var purchaseOrder = await _purchaseOrderRepository.GetPurchaseOrder(purchaseOrderId);
            if (purchaseOrder == null)
                return NotFound("PurchaseOrder not found!");
            return Ok(purchaseOrder);
        }

        [HttpGet("GetPurchaseOrdersPaginated")]
        public async Task<ActionResult<PaginatedListDTO<PurchaseOrder>>> GetPurchaseOrdersPaginated(
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "PFWBNumber",
            [FromQuery] string? sortOrder = "DESC",
            [FromQuery] string? search = ""
            )
        {
            var result = await _purchaseOrderRepository.GetPurchaseOrdersPaginated(pageSize, pageIndex, sortColumn, sortOrder, search);
            return Ok(result);
        }

        [HttpGet("GetPurchaseOrdersByDatePaginated")]
        public async Task<ActionResult<PaginatedListDTO<PurchaseOrder>>> GetPurchaseOrdersByDatePaginated(int pageSize, int pageIndex, DateTime fromDate, DateTime toDate)
        {
            var result = await _purchaseOrderRepository.GetPurchaseOrdersByDatePaginated(pageSize, pageIndex, fromDate, toDate);
            return Ok(result);
        }

        [HttpGet("GetDailyVendorSalesSummary")]
        public async Task<ActionResult<DailyVendorSalesSummaryDTO>> GetDailyVendorSalesSummary(DateTime currentDate)
        {
            var totalSales = await _purchaseOrderRepository.GetDailyVendorSalesSummary(currentDate);
            return Ok(totalSales);
        }

        [HttpGet("GetDailyVendorSalesSummaryByDate")]
        public async Task<ActionResult<DailyVendorSalesSummaryDTO>> GetDailyVendorSalesSummaryByDate(DateTime fromDate, DateTime toDate)
        {
            var totalSales = await _purchaseOrderRepository.GetDailyVendorSalesSummaryByDate(fromDate, toDate);
            return Ok(totalSales);
        }
        #endregion

        #region Save Data
        [HttpPost("CreatePurchaseOrder")]
        public async Task<ActionResult<bool>> CreatePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            var result = await _purchaseOrderRepository.Create(purchaseOrder);
            return Ok(result);
        }

        [HttpPut("UpdatePurchaseOrder")]
        public async Task<ActionResult<bool>> UpdatePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            var result = await _purchaseOrderRepository.Update(purchaseOrder);
            return Ok(result);
        }

        [HttpPut("SoftDeletePurchaseOrderDetail")]
        public async Task<ActionResult<bool>> SoftDeletePurchaseOrderDetail(PurchaseOrderDetail purchaseOrderDetail)
        {
            var result = await _purchaseOrderRepository.SoftDeletePurchaseOrderDetail(purchaseOrderDetail);
            return Ok(result);
        }

        [HttpDelete("DeletePurchaseOrder")]
        public async Task<ActionResult<bool>> DeletePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            var purchaseOrderList = await _purchaseOrderRepository.Delete(purchaseOrder);
            return Ok(purchaseOrderList);
        }
        #endregion
    }
}
