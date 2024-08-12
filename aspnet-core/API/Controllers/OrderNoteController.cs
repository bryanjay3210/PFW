using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Infrastucture;
using Infrastucture.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderNoteController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IOrderNoteRepository _orderNoteRepository;

        public OrderNoteController(DataContext dataContext, IOrderNoteRepository orderNoteRepository)
        {
            _dataContext = dataContext;
            _orderNoteRepository = orderNoteRepository;
        }

        #region Get Data
        [HttpGet("GetOrderNotes")]
        public async Task<ActionResult<List<OrderNote>>> GetOrderNotes()
        {
            return Ok(await _orderNoteRepository.GetOrderNotes());
        }

        //[HttpGet("GetOrderNoteById")]
        //public async Task<ActionResult<OrderNote>> GetOrderNoteById(int orderNoteId)
        //{
        //    var orderNote = await _orderNoteRepository.GetOrderNote(orderNoteId);
        //    if (orderNote == null)
        //        return NotFound("OrderNote not found!");
        //    return Ok(orderNote);
        //}

        [HttpGet("GetOrderNotesByOrderId")]
        public async Task<ActionResult<List<OrderNote>>> GetOrderNotesByOrderId(int orderId)
        {
            var orderNotes = await _orderNoteRepository.GetOrderNotesByOrderId(orderId);
            if (orderNotes == null)
                return NotFound("OrderNotes not found!");

            return Ok(orderNotes);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateOrderNote")]
        public async Task<ActionResult<List<OrderNote>>> CreateOrderNote(OrderNote orderNote)
        {
            var orderNoteList = await _orderNoteRepository.Create(orderNote);
            return Ok(orderNoteList);
        }

        [HttpPut("UpdateOrderNote")]
        public async Task<ActionResult<List<OrderNote>>> UpdateOrderNote(OrderNote orderNote)
        {
            var orderNoteList = await _orderNoteRepository.Update(orderNote);
            return Ok(orderNoteList);
        }

        [HttpDelete("DeleteOrderNote")]
        public async Task<ActionResult<List<OrderNote>>> DeleteOrderNote(List<int> orderNoteIds)
        {
            var orderNoteList = await _orderNoteRepository.Delete(orderNoteIds);
            return Ok(orderNoteList);
        }
        #endregion
    }
}
