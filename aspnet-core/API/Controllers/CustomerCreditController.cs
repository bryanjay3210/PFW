using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO.Paginated;
using Domain.DomainModel.Helper;
using Domain.DomainModel.Interface;
using Infrastucture;
using Infrastucture.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerCreditController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly ICustomerCreditRepository _customerCreditRepository;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _imageUrlBase;
        private readonly bool _enableSync;


        public CustomerCreditController(DataContext dataContext, ICustomerCreditRepository customerCreditRepository, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _customerCreditRepository = customerCreditRepository;
            _configuration = configuration;
            _connectionString = _configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
            _imageUrlBase = _configuration.GetValue<string>("ImageUrlBase");
            _enableSync = _configuration.GetValue<string>("EnableSync").ToLower() == "true";
        }

        #region Get Data

        [HttpGet("GetCustomerCreditById")]
        public async Task<ActionResult<CustomerCredit>> GetCustomerCreditById(int customerCreditId)
        {
            var customerCredit = await _customerCreditRepository.GetCustomerCredit(customerCreditId);
            if (customerCredit == null)
                return NotFound("CustomerCredit not found!");
            return Ok(customerCredit);
        }

        [HttpGet("GetCustomerCreditsByCustomerId")]
        public async Task<ActionResult<List<CustomerCredit>>> GetCustomerCreditsByCustomerId(int customerId)
        {
            var result = await _customerCreditRepository.GetCustomerCreditsByCustomerId(customerId);
            return Ok(result);
        }
        
        #endregion

        #region Save Data
        [HttpPost("CreateCustomerCredit")]
        public async Task<ActionResult<CustomerCredit>> CreateCustomerCredit(CustomerCredit customerCredit)
        {
            try
            {
                var result = await _customerCreditRepository.Create(customerCredit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateCustomerCredit")]
        public async Task<ActionResult<bool>> UpdateCustomerCredit(CustomerCredit customerCredit)
        {
            try 
            {
                var result = await _customerCreditRepository.Update(customerCredit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteCustomerCredit")]
        public async Task<ActionResult<List<CustomerCredit>>> DeleteCustomerCredit(List<int> customerCreditIds)
        {
            try
            {
                var customerCreditList = await _customerCreditRepository.Delete(customerCreditIds);
                return Ok(customerCreditList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
