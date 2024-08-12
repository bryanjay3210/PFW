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
    public class CustomerController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerNoteRepository _customerNoteRepository;

        public CustomerController(DataContext dataContext, ICustomerRepository customerRepository, ICustomerNoteRepository customerNoteRepository)
        {
            _dataContext = dataContext;
            _customerRepository = customerRepository;
            _customerNoteRepository = customerNoteRepository;
        }

        #region Get Data
        [HttpGet("GetCustomers")]
        public async Task<ActionResult<List<Customer>>> GetCustomers()
        {
            var result = await _customerRepository.GetCustomers();
            if (result == null)
                return NotFound("Customers not found!");

            return Ok(result);
        }

        [HttpGet("GetCustomersPaginated")]
        public async Task<ActionResult<PaginatedListDTO<Customer>>> GetCustomersPaginated(
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "CustomerName",
            [FromQuery] string? sortOrder = "ASC",
            [FromQuery] string? search = ""
            )
        {
            search = !string.IsNullOrEmpty(search) ? search.Replace("<--->", "&").Trim() : null;
            var result = await _customerRepository.GetCustomersPaginated(pageSize, pageIndex, sortColumn, sortOrder, search);
            return Ok(result);
        }

        [HttpGet("GetCustomersList")]
        public async Task<ActionResult<List<CustomerDTO>>> GetCustomersList()
        {
            var result = await _customerRepository.GetCustomersList();
            if (result == null)
                return NotFound("Customers not found!");

            return Ok(result);
        }

        [HttpGet("GetCustomersListPaginated")]
        public async Task<ActionResult<PaginatedListDTO<CustomerDTO>>> GetCustomersListPaginated(
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "CustomerName",
            [FromQuery] string? sortOrder = "ASC",
            [FromQuery] string? search = ""
            )
        {
            search = !string.IsNullOrEmpty(search) ? search.Replace("<--->", "&").Trim() : null;
            var result = await _customerRepository.GetCustomersListPaginated(pageSize, pageIndex, sortColumn, sortOrder, search);
            return Ok(result);
        }

        [HttpGet("GetReportCustomersListPaginated")]
        public async Task<ActionResult<PaginatedListDTO<CustomerDTO>>> GetReportCustomersListPaginated(
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "CustomerName",
            [FromQuery] string? sortOrder = "ASC",
            [FromQuery] string? search = "",
            [FromQuery] int? searchPaymentTermId = 0,
            [FromQuery] string? searchState = ""
            )
        {
            search = !string.IsNullOrEmpty(search) ? search.Replace("<--->", "&").Trim() : null;
            var result = await _customerRepository.GetReportCustomersListPaginated(pageSize, pageIndex, sortColumn, sortOrder, search, searchPaymentTermId, searchState);
            return Ok(result);
        }

        [HttpGet("GetCustomer")]
        public async Task<ActionResult<Customer>> GetCustomer(int customerId)
        {
            var customer = await _customerRepository.GetCustomer(customerId);
            if (customer == null)
                return NotFound("Customer not found!");

            return Ok(customer);
        }

        [HttpGet("GetCustomerById")]
        public async Task<ActionResult<CustomerDTO?>> GetCustomerById(int customerId)
        {
            var customer = await _customerRepository.GetCustomerById(customerId);
            if (customer == null)
                return NotFound("Customer not found!");

            return Ok(customer);
        }

        [HttpGet("GetCustomerEmailsById")]
        public async Task<ActionResult<List<CustomerEmailDTO>?>> GetCustomerEmailsById(int customerId)
        {
            var customerEmails = await _customerRepository.GetCustomerEmailsById(customerId);
            if (customerEmails == null)
                return NotFound("Customer Email not found!");

            return Ok(customerEmails);
        }

        [HttpGet("GetCustomerByAccountNumber")]
        public async Task<ActionResult<CustomerDTO?>> GetCustomerByAccountNumber(int accountNumber)
        {
            var customer = await _customerRepository.GetCustomerByAccountNumber(accountNumber);
            if (customer == null)
                return NotFound("Customer not found!");

            return Ok(customer);
        }

        // CustomerNote
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
                return NotFound("Customer not found!");
            return Ok(customerNote);
        }
        #endregion

        #region Save Data
        // Customer
        [HttpPost("CreateCustomer")]
        public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
        {
            var maxAccountNumber = _dataContext.Customers.Count() > 0 ? _dataContext.Customers.Max(e => e.AccountNumber) + 1 : 1;
            customer.AccountNumber = maxAccountNumber;
            var newCustomer = await _customerRepository.Create(customer);

            if (newCustomer == null)
                return NotFound("New customer not created!");

            return Ok(newCustomer);
        }

        [HttpPut("UpdateCustomer")]
        public async Task<ActionResult<bool>> UpdateCustomer(Customer customer)
        {
            var result = await _customerRepository.Update(customer);
            if (!result)
                return BadRequest("Update Customer Error!");

            return Ok(result);
        }

        [HttpDelete("DeleteCustomer")]
        public async Task<ActionResult<List<Customer>>> DeleteCustomer(List<int> customerIds)
        {
            var customerList = await _customerRepository.Delete(customerIds);

            //if (customerList == null)
            //    return NotFound("Error encountered when deleting customer!");

            return Ok(customerList);
        }

        // CustomerNote
        [HttpPost("CreateCustomerNote")]
        public async Task<ActionResult<List<CustomerNote>>> CreateCustomerNote(CustomerNote customerNote)
        {
            var customerNoteList = await _customerNoteRepository.Create(customerNote);

            //if (customerList == null)
            //    return NotFound("New customer not created!");

            return Ok(customerNoteList);
        }

        [HttpPut("UpdateCustomerNote")]
        public async Task<ActionResult<List<CustomerNote>>> UpdateCustomerNote(CustomerNote customerNote)
        {
            var customerNoteList = await _customerNoteRepository.Update(customerNote);

            //if (customerList == null)
            //    return NotFound("Error encountered while updating customer!");

            return Ok(customerNoteList);
        }

        [HttpDelete("DeleteCustomerNote")]
        public async Task<ActionResult<List<CustomerNote>>> DeleteCustomerNote(List<int> customerNoteIds)
        {
            var customerNoteList = await _customerNoteRepository.Delete(customerNoteIds);

            //if (customerList == null)
            //    return NotFound("Error encountered when deleting customer!");

            return Ok(customerNoteList);
        }
        #endregion
    }
}
