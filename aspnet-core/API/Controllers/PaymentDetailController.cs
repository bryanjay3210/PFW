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
    public class PaymentDetailController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IPaymentDetailRepository _paymentDetailRepository;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _imageUrlBase;
        private readonly bool _enableSync;


        public PaymentDetailController(DataContext dataContext, IPaymentDetailRepository paymentDetailRepository, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _paymentDetailRepository = paymentDetailRepository;
            _configuration = configuration;
            _connectionString = _configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
            _imageUrlBase = _configuration.GetValue<string>("ImageUrlBase");
            _enableSync = _configuration.GetValue<string>("EnableSync").ToLower() == "true";
        }

        #region Get Data

        [HttpGet("GetPaymentDetailById")]
        public async Task<ActionResult<PaymentDetail>> GetPaymentDetailById(int paymentDetailId)
        {
            var paymentDetail = await _paymentDetailRepository.GetPaymentDetail(paymentDetailId);
            if (paymentDetail == null)
                return NotFound("PaymentDetail not found!");
            return Ok(paymentDetail);
        }

        [HttpGet("GetPaymentDetailsByPaymentId")]
        public async Task<ActionResult<List<PaymentDetail>>> GetPaymentDetailsByPaymentId(int paymentId)
        {
            var result = await _paymentDetailRepository.GetPaymentDetailsByPaymentId(paymentId);
            return Ok(result);
        }
        
        #endregion

        #region Save Data
        [HttpPost("CreatePaymentDetail")]
        public async Task<ActionResult<PaymentDetail>> CreatePaymentDetail(PaymentDetail paymentDetail)
        {
            try
            {
                var result = await _paymentDetailRepository.Create(paymentDetail);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdatePaymentDetail")]
        public async Task<ActionResult<bool>> UpdatePaymentDetail(PaymentDetail paymentDetail)
        {
            try 
            {
                var result = await _paymentDetailRepository.Update(paymentDetail);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeletePaymentDetail")]
        public async Task<ActionResult<List<PaymentDetail>>> DeletePaymentDetail(List<int> paymentDetailIds)
        {
            try
            {
                var paymentDetailList = await _paymentDetailRepository.Delete(paymentDetailIds);
                return Ok(paymentDetailList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
