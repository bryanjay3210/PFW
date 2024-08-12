using Domain.DomainModel.Entity.DTO.Paginated;
using Domain.DomainModel.Entity;
using Infrastucture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.DomainModel.Interface;
using Domain.DomainModel.Entity.DTO;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IOrderRepository _orderRepository;

        public ReportController(DataContext dataContext, IOrderRepository orderRepository)
        {
            _dataContext = dataContext;
            _orderRepository = orderRepository;
        }

        #region Get Data
        [HttpGet("GetAgingBalanceReport")]
        public async Task<ActionResult<List<AgingBalanceReportDTO>>> GetAgingBalanceReport(DateTime reportDate)
        {
            var result = new List<AgingBalanceReportDTO>();
            result = await _orderRepository.GetAgingBalanceReport(reportDate);
            return Ok(result);
        }

        [HttpPut("GetStatementReport")]
        public async Task<ActionResult<List<StatementReportDTO>>> GetStatementReport(DateTime reportDate, int paymentTermId, List<int> customerIds)
        {
            List<StatementReportDTO> result = await _orderRepository.GetStatementReport(reportDate, paymentTermId, customerIds);
            if (result == null)
                return NotFound("Customers not found!");

            return Ok(result);
        }

        [HttpPut("GetStatementTotalReport")]
        public async Task<ActionResult<List<StatementTotalReportDTO>>> GetStatementTotalReport(DateTime reportDate, int paymentTermId, List<int> customerIds)
        {
            List<StatementTotalReportDTO> result = await _orderRepository.GetStatementTotalReport(reportDate, paymentTermId, customerIds);
            if (result == null)
                return NotFound("Customers not found!");

            return Ok(result);
        }

        [HttpPut("UpdatePrintedInvoice")]
        public async Task<ActionResult<bool>> UpdatePrintedInvoice(List<int> orderIds)
        {
            bool result = await _orderRepository.UpdatePrintedInvoice(orderIds);
            if (!result)
                return NotFound("Failed updating orders IsPrinted!");

            return Ok(result);
        }

        #endregion
    }
}
