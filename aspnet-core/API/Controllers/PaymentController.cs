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
    public class PaymentController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _imageUrlBase;
        private readonly bool _enableSync;


        public PaymentController(DataContext dataContext, IPaymentRepository paymentRepository, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _paymentRepository = paymentRepository;
            _configuration = configuration;
            _connectionString = _configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
            _imageUrlBase = _configuration.GetValue<string>("ImageUrlBase");
            _enableSync = _configuration.GetValue<string>("EnableSync").ToLower() == "true";
        }

        #region Get Data

        [HttpGet("GetPaymentsPaginated")]
        public async Task<ActionResult<PaginatedListDTO<Payment>>> GetPaymentsPaginated(
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "PaymentNumber",
            [FromQuery] string? sortOrder = "DESC",
            [FromQuery] string? search = ""
            )
        {
            var result = await _paymentRepository.GetPaymentsPaginated(pageSize, pageIndex, sortColumn, sortOrder, search);
            return Ok(result);
        }

        [HttpGet("GetPaymentsByDatePaginated")]
        public async Task<ActionResult<PaginatedListDTO<Payment>>> GetPaymentsByDatePaginated(int pageSize, int pageIndex, DateTime fromDate, DateTime toDate)
        {
            var result = await _paymentRepository.GetPaymentsByDatePaginated(pageSize, pageIndex, fromDate, toDate);
            return Ok(result);
        }

        [HttpGet("GetPaymentById")]
        public async Task<ActionResult<Payment>> GetPaymentById(int paymentId)
        {
            var payment = await _paymentRepository.GetPayment(paymentId);
            if (payment == null)
                return NotFound("Payment not found!");
            return Ok(payment);
        }

        [HttpGet("GetPaymentHistoryByOrderNumber")]
        public async Task<ActionResult<PaymentHistoryDTO>> GetPaymentHistoryByOrderNumber(int orderNumber)
        {
            var paymentHistoryList = await _paymentRepository.GetPaymentHistoryByOrderNumber(orderNumber);
            //if (paymentHistoryList.Count == 0)
            //    return NotFound("Payment History not found!");
            return Ok(paymentHistoryList);
        }

        [HttpGet("GetDailyPaymentSummary")]
        public async Task<ActionResult<DailyPaymentSummaryDTO>> GetDailyPaymentSummary(DateTime currentDate)
        {
            var totalPayments = await _paymentRepository.GetDailyPaymentSummary(currentDate);
            if (totalPayments == null)
            {
                return NotFound();
            }
            return Ok(totalPayments);
        }

        [HttpGet("GetPaymentSummaryByDate")]
        public async Task<ActionResult<DailyPaymentSummaryDTO>> GetPaymentSummaryByDate(DateTime fromDate, DateTime toDate)
        {
            var totalPayments = await _paymentRepository.GetPaymentSummaryByDate(fromDate, toDate);
            if (totalPayments == null)
            {
                return NotFound();
            }
            return Ok(totalPayments);
        }

        
        #endregion

        #region Save Data
        [HttpPost("CreatePayment")]
        public async Task<ActionResult<Payment>> CreatePayment(Payment payment)
        {
            try
            {
                var result = await _paymentRepository.Create(payment);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateRefund")]
        public async Task<ActionResult<Payment>> CreateRefund(Payment payment)
        {
            try
            {
                var result = await _paymentRepository.CreateRefund(payment);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdatePayment")]
        public async Task<ActionResult<bool>> UpdatePayment(Payment payment)
        {
            try 
            {
                var result = await _paymentRepository.Update(payment);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeletePayment")]
        public async Task<ActionResult<List<Payment>>> DeletePayment(List<int> paymentIds)
        {
            try
            {
                var paymentList = await _paymentRepository.Delete(paymentIds);
                return Ok(paymentList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
