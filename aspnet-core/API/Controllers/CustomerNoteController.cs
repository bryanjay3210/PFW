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
    public class CustomerNoteController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly ICustomerNoteRepository _customerNoteRepository;

        public CustomerNoteController(DataContext dataContext, ICustomerNoteRepository customerNoteRepository)
        {
            _dataContext = dataContext;
            _customerNoteRepository = customerNoteRepository;
        }

        #region Get Data
        [HttpGet("GetCustomerNotes")]
        public async Task<ActionResult<List<CustomerNote>>> GetCustomerNotes()
        {
            return Ok(await _customerNoteRepository.GetCustomerNotes());
        }

        [HttpGet("GetCustomerNoteById")]
        public async Task<ActionResult<CustomerNote>> GetCustomerNoteById(int customerNoteId)
        {
            var customerNote = await _customerNoteRepository.GetCustomerNote(customerNoteId);
            if (customerNote == null)
                return NotFound("CustomerNote not found!");
            return Ok(customerNote);
        }

        [HttpGet("GetCustomerNotesByCustomerId")]
        public async Task<ActionResult<List<CustomerNote>>> GetCustomerNotesByCustomerId(int customerId)
        {
            var customerNotes = await _customerNoteRepository.GetCustomerNotesByCustomerId(customerId);
            if (customerNotes == null)
                return NotFound("CustomerNotes not found!");

            return Ok(customerNotes);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateCustomerNote")]
        public async Task<ActionResult<List<CustomerNote>>> CreateCustomerNote(CustomerNote customerNote)
        {
            var customerNoteList = await _customerNoteRepository.Create(customerNote);
            return Ok(customerNoteList);
        }

        [HttpPut("UpdateCustomerNote")]
        public async Task<ActionResult<List<CustomerNote>>> UpdateCustomerNote(CustomerNote customerNote)
        {
            var customerNoteList = await _customerNoteRepository.Update(customerNote);
            return Ok(customerNoteList);
        }

        [HttpDelete("DeleteCustomerNote")]
        public async Task<ActionResult<List<CustomerNote>>> DeleteCustomerNote(List<int> customerNoteIds)
        {
            var customerNoteList = await _customerNoteRepository.Delete(customerNoteIds);
            return Ok(customerNoteList);
        }
        #endregion
    }
}
