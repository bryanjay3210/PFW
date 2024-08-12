using API.Helpers;
using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Interface;
using Infrastucture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Email;
using System.Net.Http.Headers;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IEmailService _emailService;
        private readonly IOrderRepository _orderRepository;
        private readonly IContactRepository _contactRepository;
        private readonly EmailHelper _emailHelper;

        public EmailController(DataContext dataContext, IEmailService emailService, IOrderRepository orderRepository, IContactRepository contactRepository, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _emailService = emailService;
            _orderRepository = orderRepository;
            _contactRepository = contactRepository;
            _emailHelper = new EmailHelper(dataContext, emailService, configuration);
        }
        [HttpPost("SendUserRegistrationEmail")]
        public ActionResult<bool> SendUserRegistrationEmail(RegisterUserDTO user)
        {
            try
            {
                var result = _emailService.SendUserRegistrationEmail(user);
                if (!result) return BadRequest("Email registration failed.");

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("SendDriverLogEmails"), DisableRequestSizeLimit]
        public async Task<IActionResult> SendDriverLogEmails(DriverLog driverLog)
        {
            try
            {
                foreach (var log in driverLog.DriverLogDetails.GroupBy(e => e.OrderId).Select(g => g.First()).ToList())
                {
                    var order = await _dataContext.Orders.FirstOrDefaultAsync(e => e.Id == log.OrderId);
                    if (order != null && order.OrderStatusId != 9)
                    {
                        var orderDetails = await _dataContext.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).ToListAsync();
                        var customer = await _dataContext.Customers.FirstOrDefaultAsync(e => e.Id == order.CustomerId);
                        var contacts = await _contactRepository.GetContactsByCustomerId(order.CustomerId);

                        if (contacts != null)
                        {
                            var contactList = contacts.Where(e => e.IsEmailOrder).ToList();

                            if (contactList != null)
                            {
                                await _emailHelper.SendOrderEmailByContacts(order, orderDetails, customer, contactList);
                            }
                        }
                    }
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("SendStatementEmails"), DisableRequestSizeLimit]
        public async Task<IActionResult> SendStatementEmails(StatementEmailParamDTO statementEmailParam) //DateTime reportDate, int paymentTermId, List<int> customerIds, DateTime dueDate)
        {
            try
            {
                var reportDate = statementEmailParam.ReportDate;
                var dueDate = statementEmailParam.DueDate;
                var paymentTermId = statementEmailParam.PaymentTermId;
                var customerIds = statementEmailParam.CustomerIds;

                var statements = await _orderRepository.GetStatementReport(reportDate, paymentTermId, customerIds);

                foreach (var statement  in statements)
                {
                    var customer = await _dataContext.Customers.FirstOrDefaultAsync(e => e.Id == statement.CustomerId);

                    if (customer != null)
                    {
                        await _emailHelper.SendStatementEmail(customer, statement, reportDate, dueDate);
                    }
                }

                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("SendOrderEmail"), DisableRequestSizeLimit]
        public async Task<IActionResult> SendOrderEmail(int orderId, string contactName, string email)
        {
            try
            {
                var result = await _emailHelper.SendOrderEmail(orderId, contactName, email);

                if (!result)
                {
                    return BadRequest("Error encountered while sending email!");
                }

                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpGet("SendStatementEmail"), DisableRequestSizeLimit]
        public async Task<IActionResult> SendStatementEmail(DateTime reportDate, int paymentTermId, int customerId, DateTime dueDate)
        {
            try
            {
                // Get Customer & Statement
                var customer = await _dataContext.Customers.FirstOrDefaultAsync(e => e.Id == customerId);
                var statements = await _orderRepository.GetStatementReport(reportDate, paymentTermId, new List<int>() { customerId });

                if (customer != null)
                {
                    var statement = statements[0];
                    if (statement != null)
                    {
                        await _emailHelper.SendStatementEmail(customer, statement, reportDate, dueDate);
                        return Ok(true);
                    }

                    return BadRequest("Statement not found!");
                }

                return BadRequest("Customer not found!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                //var file = Request.Form.Files[0];

                //var folderName = Path.Combine("Resources", "Images");
                var folderName = Path.Combine("Resources");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
