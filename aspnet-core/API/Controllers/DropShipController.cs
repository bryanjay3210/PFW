using Domain.DomainModel.Entity.DropShip;
using Domain.DomainModel.Entity.DTO.Reports;
using Domain.DomainModel.Interface;
using Domain.DomainModel.Interface.User;
using Infrastucture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DropShipController : ControllerBase
    {
        private readonly IDropShipRepository _dropShipRepository;

        public DropShipController(IDropShipRepository dropShipRepository)
        {
            _dropShipRepository = dropShipRepository;
        }

        [HttpPost("CheckInventory")]
        public async Task<ActionResult<CheckInventoryResponse>> CheckInventory(CheckInventoryRequest request)
        {
            var result = await _dropShipRepository.CheckInventory(request);
            return Ok(result);
        }

        [HttpPost("PlaceOrder")]
        public async Task<ActionResult<PlaceOrderResponse>> PlaceOrder(PlaceOrderRequest request)
        {
            var result = await _dropShipRepository.PlaceOrder(request);
            return Ok(result);
        }

        [HttpPost("OrderStatus")]
        public async Task<ActionResult<OrderStatusResponse>> OrderStatus(OrderStatusRequest request)
        {
            var result = await _dropShipRepository.OrderStatus(request);
            return Ok(result);
        }

        //[HttpGet("GenerateBellFlowerDailyReport")]
        //public async Task<ActionResult<List<BellFlowerDailyReportDTO>>> GenerateBellFlowerDailyReport()
        //{
        //    _dropShipRepository. GenerateBellFlowerDailyReport(20, "");
        //    return Ok();
        //}
    }
}
