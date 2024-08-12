using API.Helpers;
using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.DTO.Paginated;
using Domain.DomainModel.Helper;
using Domain.DomainModel.Interface;
using Infrastucture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Service.Email;
using System.Data;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IOrderRepository _orderRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _imageUrlBase;
        private readonly bool _enableSync;
        private readonly EmailHelper _emailHelper;

        public OrderController(DataContext dataContext, IOrderRepository orderRepository, IContactRepository contactRepository, IEmailService emailService, IConfiguration configuration)
        {
            _dataContext = dataContext; // TODO: This should be removed!!!
            _orderRepository = orderRepository;
            _contactRepository = contactRepository;
            _emailService = emailService;
            _configuration = configuration;
            _connectionString = _configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
            _imageUrlBase = _configuration.GetValue<string>("ImageUrlBase");
            _enableSync = _configuration.GetValue<string>("EnableSync").ToLower() == "true";
            _emailHelper = new EmailHelper(dataContext, _emailService, configuration);
        }

        #region Get Data
        [HttpGet("GetOrders")]
        public async Task<ActionResult<List<Order>>> GetOrders()
        {
            return Ok(await _orderRepository.GetOrders());
        }

        [HttpGet("GetOrderByOrderNumber")]
        public async Task<ActionResult<Order>> GetOrderByOrderNumber(int orderNumber)
        {
            var result = await _orderRepository.GetOrderByOrderNumber(orderNumber);
            if (result == null) { return BadRequest("Order not found!"); }
            
            return Ok(result);
        }

        [HttpGet("GetOrdersByCustomerId")]
        public async Task<ActionResult<List<Order>>> GetOrdersByCustomerId(int customerId)
        {
            return Ok(await _orderRepository.GetOrdersByCustomerId(customerId));
        }

        [HttpGet("GetInvoicesByCustomerId")]
        public async Task<ActionResult<List<Order>>> GetInvoicesByCustomerId(int customerId)
        {
            return Ok(await _orderRepository.GetInvoicesByCustomerId(customerId));
        }

        [HttpPut("GetInvoicesByCustomerIds")]
        public async Task<ActionResult<List<Order>>> GetInvoicesByCustomerIds(List<int> customerIds)
        {
            return Ok(await _orderRepository.GetInvoicesByCustomerIds(customerIds));
        }

        [HttpGet("GetCreditMemoByInvoiceNumber")]
        public async Task<ActionResult<List<Order>>> GetCreditMemoByInvoiceNumber(int invoiceNumber)
        {
            return Ok(await _orderRepository.GetCreditMemoByInvoiceNumber(invoiceNumber));
        }

        [HttpGet("GetCreditMemoByCustomerId")]
        public async Task<ActionResult<List<Order>>> GetCreditMemoByCustomerId(int customerId)
        {
            return Ok(await _orderRepository.GetCreditMemoByCustomerId(customerId));
        }

        [HttpPut("GetCreditMemoByCustomerIds")]
        public async Task<ActionResult<List<Order>>> GetCreditMemoByCustomerIds(List<int> customerIds)
        {
            return Ok(await _orderRepository.GetCreditMemoByCustomerIds(customerIds));
        }

        [HttpGet("GetOrdersPaginated")]
        public async Task<ActionResult<PaginatedListDTO<Order>>> GetOrdersPaginated(
            [FromQuery] int searchType = 0,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "OrderNumber",
            [FromQuery] string? sortOrder = "DESC",
            [FromQuery] string? search = "",
            [FromQuery] int? paymentTerm = 0
            )
        {
            var result = await _orderRepository.GetOrdersPaginated(searchType, pageSize, pageIndex, sortColumn, sortOrder, search, paymentTerm);
            result.Data.ForEach(e =>
            {
                e.OrderDetails.ForEach(d =>
                {
                    d.ImageUrl = string.Format(_imageUrlBase, d.PartNumber.Trim());
                });
            });
            return Ok(result);
        }

        [HttpGet("GetOrdersByDatePaginated")]
        public async Task<ActionResult<PaginatedListDTO<Order>>> GetOrdersByDatePaginated(int searchType, int pageSize, int pageIndex, DateTime fromDate, DateTime toDate, string? search, int? paymentTerm)
        {
            var result = await _orderRepository.GetOrdersByDatePaginated(searchType, pageSize, pageIndex, fromDate, toDate, search, paymentTerm);
            return Ok(result);
        }

        //[HttpGet("GetOrdersByDatePaginated")]
        //public async Task<ActionResult<PaginatedListDTO<Order>>> GetOrdersByDatePaginated(int pageSize, int pageIndex, DateTime fromDate, DateTime toDate)
        //{
        //    var result = await _orderRepository.GetOrdersByDatePaginated(pageSize, pageIndex, fromDate, toDate);
        //    return Ok(result);
        //}

        [HttpGet("GetCustomerOrdersPaginated")]
        public async Task<ActionResult<PaginatedListDTO<Order>>> GetCustomerOrdersPaginated(
            [FromQuery] int customerId = 0,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "OrderNumber",
            [FromQuery] string? sortOrder = "DESC",
            [FromQuery] string? search = ""
            )
        {
            var result = await _orderRepository.GetCustomerOrdersPaginated(customerId, pageSize, pageIndex, sortColumn, sortOrder, search);
            result.Data.ForEach(e =>
            {
                e.OrderDetails.ForEach(d =>
                {
                    d.ImageUrl = string.Format(_imageUrlBase, d.PartNumber.Trim());
                });
            });
            return Ok(result);
        }

        [HttpGet("GetQuotesPaginated")]
        public async Task<ActionResult<PaginatedListDTO<Order>>> GetQuotesPaginated(
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "OrderNumber",
            [FromQuery] string? sortOrder = "DESC",
            [FromQuery] string? search = ""
            )
        {
            var result = await _orderRepository.GetQuotesPaginated(pageSize, pageIndex, sortColumn, sortOrder, search);
            result.Data.ForEach(e =>
            {
                e.OrderDetails.ForEach(d =>
                {
                    d.ImageUrl = string.Format(_imageUrlBase, d.PartNumber.Trim());
                });
            });
            return Ok(result);
        }

        [HttpGet("GetWebOrdersPaginated")]
        public async Task<ActionResult<PaginatedListDTO<Order>>> GetWebOrdersPaginated(
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "OrderNumber",
            [FromQuery] string? sortOrder = "DESC",
            [FromQuery] string? search = ""
            )
        {
            var result = await _orderRepository.GetWebOrdersPaginated(pageSize, pageIndex, sortColumn, sortOrder, search);
            result.Data.ForEach(e =>
            {
                e.OrderDetails.ForEach(d =>
                {
                    d.ImageUrl = string.Format(_imageUrlBase, d.PartNumber.Trim());
                });
            });
            return Ok(result);
        }

        [HttpGet("GetRGAOrdersPaginated")]
        public async Task<ActionResult<PaginatedListDTO<Order>>> GetRGAOrdersPaginated(
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "OrderNumber",
            [FromQuery] string? sortOrder = "DESC",
            [FromQuery] string? search = ""
            )
        {
            var result = await _orderRepository.GetRGAOrdersPaginated(pageSize, pageIndex, sortColumn, sortOrder, search);
            result.Data.ForEach(e =>
            {
                e.OrderDetails.ForEach(d =>
                {
                    d.ImageUrl = string.Format(_imageUrlBase, d.PartNumber.Trim());
                });
            });
            return Ok(result);
        }

        [HttpGet("GetReportOrdersListPaginated")]
        public async Task<ActionResult<PaginatedListDTO<Order>>> GetReportOrdersListPaginated(
            [FromQuery] DateTime deliveryDate = default(DateTime),
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "OrderNumber",
            [FromQuery] string? sortOrder = "ASC",
            [FromQuery] string? search = "",
            [FromQuery] string? state = "",
            [FromQuery] int? deliveryRoute = 0
            )
        {
            var result = await _orderRepository.GetReportOrdersListPaginated(deliveryDate, pageSize, pageIndex, sortColumn, sortOrder, search, state, deliveryRoute);
            return Ok(result);
        }

        [HttpGet("GetOrderById")]
        public async Task<ActionResult<Order>> GetOrderById(int orderId)
        {
            var order = await _orderRepository.GetOrder(orderId);
            if (order == null)
                return NotFound("Order not found!");
            return Ok(order);
        }

        [HttpGet("GetDailySalesSummary")]
        public async Task<ActionResult<DailySalesSummaryDTO>> GetDailySalesSummary(DateTime currentDate)
        {
            var totalSales = await _orderRepository.GetDailySalesSummary(currentDate);
            return Ok(totalSales);
        }

        [HttpPut("GetCustomerSales")]
        public async Task<ActionResult<List<CustomerSalesAmountDTO>>> GetCustomerSales(CustomerSalesFilterDTO filter)
        {
            var customerSales = await _orderRepository.GetCustomerSales(filter);
            return Ok(customerSales);
        }

        [HttpGet("GetDailySalesSummaryByDate")]
        public async Task<ActionResult<DailySalesSummaryDTO>> GetDailySalesSummaryByDate(DateTime fromDate, DateTime toDate)
        {
            var totalSales = await _orderRepository.GetDailySalesSummaryByDate(fromDate, toDate);
            return Ok(totalSales);
        }

        [HttpPut("GetDiscountsByInvoiceNumber")]
        public async Task<ActionResult<List<Order>>> GetDiscountsByInvoiceNumber(int invoiceNumber, List<string> partNumbers)
        {
            return Ok(await _orderRepository.GetDiscountsByInvoiceNumber(invoiceNumber, partNumbers));
        }

        [HttpGet("GetDeliverySummary")]
        public async Task<ActionResult<DeliverySummaryDTO>> GetDeliverySummary(DateTime currentDate)
        {
            var result = await _orderRepository.GetDeliverySummary(currentDate);
            return Ok(result);
        }

        #endregion

        #region Save Data
        [HttpPost("VoidOrder")]
        public async Task<ActionResult<bool>> VoidOrder(Order order)
        {
            try
            {
                var result = await _orderRepository.VoidOrder(order);

                if (!result)
                {
                    return BadRequest("An error encountered while voiding the order.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("DeleteRGAOrder")]
        public async Task<ActionResult<bool>> DeleteRGAOrder(Order order)
        {
            try
            {
                var result = await _orderRepository.DeleteRGAOrder(order);

                if (!result)
                {
                    return BadRequest("An error encountered while voiding the RGA order.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("ConvertQuoteToOrder")]
        public async Task<ActionResult<CreateOrderResult>> ConvertQuoteToOrder(Order order)
        {
            try
            {
                var result = new CreateOrderResult();
                result.IsQuote = order.IsQuote;

                var maxOrderNumber = _dataContext.Orders.Count() > 0 ? await _dataContext.Orders.MaxAsync(e => e.OrderNumber) + 1 : 1;
                order.OrderNumber = maxOrderNumber;
                var maxInvoiceNumber = _dataContext.Orders.Count() > 0 ? await _dataContext.Orders.MaxAsync(e => e.InvoiceNumber) + 1 : 1;
                order.InvoiceNumber = maxInvoiceNumber;

                result.OrderResult = await _orderRepository.ConvertQuoteToOrder(order);

                if (result.OrderResult)
                {
                    result.Order = order;
                    result.OrderNumber = order.OrderNumber;

                    if (_enableSync)
                    {
                        result.SyncResult = await SyncOrder(order.OrderNumber != null ? order.OrderNumber.Value : 0);
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateOrder")]
        public async Task<ActionResult<CreateOrderResult>> CreateOrder(Order order)
        {
            try
            {
                var result = new CreateOrderResult();
                result.IsQuote = order.IsQuote;

                if (!order.IsQuote)
                {
                    var maxOrderNumber = _dataContext.Orders.Count() > 0 ? await _dataContext.Orders.MaxAsync(e => e.OrderNumber) + 1 : 1;
                    order.OrderNumber = maxOrderNumber;
                    var maxInvoiceNumber = _dataContext.Orders.Count() > 0 ? await _dataContext.Orders.MaxAsync(e => e.InvoiceNumber) + 1 : 1;
                    order.InvoiceNumber = maxInvoiceNumber;
                }
                else
                {
                    var maxQuoteNumber = _dataContext.Orders.Count() > 0 ? await _dataContext.Orders.MaxAsync(e => e.QuoteNumber) + 1 : 1;
                    order.QuoteNumber = maxQuoteNumber;
                }
                
                result.Order = await _orderRepository.Create(order);
                result.OrderResult = order.Id > 0;

                if (result.OrderResult)
                {
                    if (result.IsQuote)
                    {
                        result.QuoteNumber = order.QuoteNumber;
                    }
                    else
                    {
                        result.OrderNumber = order.OrderNumber;

                        if (_enableSync)
                        {
                            result.SyncResult = await SyncOrder(order.OrderNumber != null ? order.OrderNumber.Value : 0);
                        }
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateCreditMemo")]
        public async Task<ActionResult<bool>> CreateCreditMemo(Order order)
        {
            try
            {
                bool result = false;
                var maxOrderNumber = _dataContext.Orders.Count() > 0 ? await _dataContext.Orders.MaxAsync(e => e.OrderNumber) + 1 : 1;
                order.OrderNumber = maxOrderNumber;
                var maxInvoiceNumber = _dataContext.Orders.Count() > 0 ? await _dataContext.Orders.MaxAsync(e => e.InvoiceNumber) + 1 : 1;
                order.InvoiceNumber = maxInvoiceNumber;

                result = await _orderRepository.CreateCreditMemo(order);

                if (!result) return BadRequest();

                // Email Credit Memo
                var orderDetails = await _dataContext.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).ToListAsync();
                var customer = await _dataContext.Customers.FirstOrDefaultAsync(e => e.Id == order.CustomerId);

                var contacts = await _contactRepository.GetContactsByCustomerId(order.CustomerId);
                
                if (contacts != null)
                {
                    var contactList = contacts.Where(e => e.IsEmailCreditMemo).ToList();

                    if (contactList != null)
                    {
                        await _emailHelper.SendOrderEmailByContacts(order, orderDetails, customer, contactList);
                        return Ok(result);
                    }

                    return BadRequest("Contacts not found!");
                }

                return BadRequest("Contacts not found!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateRGA")]
        public async Task<ActionResult<bool>> CreateRGA(Order order)
        {
            try
            {
                bool result = false;
                var maxOrderNumber = _dataContext.Orders.Count() > 0 ? await _dataContext.Orders.MaxAsync(e => e.OrderNumber) + 1 : 1;
                order.OrderNumber = maxOrderNumber;
                var maxInvoiceNumber = _dataContext.Orders.Count() > 0 ? await _dataContext.Orders.MaxAsync(e => e.InvoiceNumber) + 1 : 1;
                order.InvoiceNumber = maxInvoiceNumber;

                result = await _orderRepository.CreateRGA(order);

                if (!result) return BadRequest();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateDiscount")]
        public async Task<ActionResult<bool>> CreateDiscount(Order order)
        {
            try
            {
                bool result = false;
                var maxOrderNumber = _dataContext.Orders.Count() > 0 ? await _dataContext.Orders.MaxAsync(e => e.OrderNumber) + 1 : 1;
                order.OrderNumber = maxOrderNumber;
                var maxInvoiceNumber = _dataContext.Orders.Count() > 0 ? await _dataContext.Orders.MaxAsync(e => e.InvoiceNumber) + 1 : 1;
                order.InvoiceNumber = maxInvoiceNumber;

                result = await _orderRepository.CreateDiscount(order);

                if (!result) return BadRequest();

                // Email Credit Memo
                var orderDetails = await _dataContext.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).ToListAsync();
                var customer = await _dataContext.Customers.FirstOrDefaultAsync(e => e.Id == order.CustomerId);

                var contacts = await _contactRepository.GetContactsByCustomerId(order.CustomerId);

                if (contacts != null)
                {
                    var contactList = contacts.Where(e => e.IsEmailCreditMemo).ToList();

                    if (contactList != null)
                    {
                        await _emailHelper.SendOrderEmailByContacts(order, orderDetails, customer, contactList);
                        return Ok(result);
                    }

                    return BadRequest("Contacts not found!");
                }

                return BadRequest("Contacts not found!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateOverpayment")]
        public async Task<ActionResult<bool>> CreateOverpayment(OverpaymentParameterDTO param)
        {
            try
            {
                bool result = false;
                result = await _orderRepository.CreateOverpayment(param);

                if (!result) return BadRequest();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private async Task<bool> SyncOrder(int orderNumber)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        Int32 rowsAffected;

                        cmd.CommandText = "PRC_Sync_Order_To_Old_System";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@OrderNumber", orderNumber));
                        cmd.Connection = sqlConnection;

                        sqlConnection.Open();

                        rowsAffected = await cmd.ExecuteNonQueryAsync();
                        
                        return true;
                    }
                }
            }
            catch (Exception ex) 
            {
                return false;
            }
        }

        [HttpPut("UpdateOrder")]
        public async Task<ActionResult<bool>> UpdateOrder(Order order)
        {
            try 
            {
                var result = await _orderRepository.Update(order);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateOrderStatus")]
        public async Task<ActionResult<bool>> UpdateOrderStatus(Order order)
        {
            try
            {
                var result = await _orderRepository.UpdateOrderStatus(order);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateOrderInspectedCode")]
        public async Task<ActionResult<bool>> UpdateOrderInspectedCode(Order order)
        {
            try
            {
                var result = await _orderRepository.UpdateOrderInspectedCode(order);

                if (result)
                {
                    //// Get Order Details
                    //var origOrderDetails = await _dataContext.OrderDetails.AsNoTracking().Where(e => e.OrderId == order.Id &&
                    //        e.RGAInspectedCode != null && e.RGAInspectedCode > 0 && e.RGAInspectedCode != 5 && !e.IsCreditMemoCreated).ToListAsync();

                    //if (origOrderDetails != null)
                    //{
                    //    order.OrderDetails = origOrderDetails;
                    //}

                    var origOrder = await _dataContext.Orders.AsNoTracking().FirstOrDefaultAsync(e => e.Id == order.Id);
                    if (origOrder != null)
                    {
                        var origOrderDetails = await _dataContext.OrderDetails.AsNoTracking().Where(e => e.OrderId == origOrder.Id &&
                            e.RGAInspectedCode != null && e.RGAInspectedCode > 0 && e.RGAInspectedCode != 5 && !e.IsCreditMemoCreated).ToListAsync();

                        if (origOrderDetails != null)
                        {
                            origOrder.OrderDetails = origOrderDetails;
                        }
                    }


                    #region Prepare Credit Memo
                    var creditMemoStr = JsonConvert.SerializeObject(origOrder, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                    var creditMemo =  JsonConvert.DeserializeObject<Order>(creditMemoStr);


                    if (creditMemo == null) return Ok(false);

                    creditMemo.Id = 0;
                    creditMemo.OrderStatusId = 5;
                    creditMemo.OrderStatusName = "Credit Memo";

                    creditMemo.CreatedDate = creditMemo.ModifiedDate != null ? creditMemo.ModifiedDate.Value.ToUniversalTime() : DateTime.Now.ToUniversalTime();
                    creditMemo.OrderDate = creditMemo.CreatedDate.ToUniversalTime();

                    var maxOrderNumber = _dataContext.Orders.Count() > 0 ? await _dataContext.Orders.MaxAsync(e => e.OrderNumber) + 1 : 1;
                    creditMemo.OrderNumber = maxOrderNumber;
                    var maxInvoiceNumber = _dataContext.Orders.Count() > 0 ? await _dataContext.Orders.MaxAsync(e => e.InvoiceNumber) + 1 : 1;
                    creditMemo.InvoiceNumber = maxInvoiceNumber;

                    foreach (var orderDetail in creditMemo.OrderDetails)
                    {
                        orderDetail.Id = 0;
                        orderDetail.UnitCost = orderDetail.UnitCost * -1;
                        orderDetail.DiscountedPrice = orderDetail.DiscountedPrice * -1;
                        orderDetail.TotalAmount = orderDetail.TotalAmount * -1;
                        orderDetail.WholesalePrice = orderDetail.WholesalePrice * -1;
                        orderDetail.RestockingAmount = orderDetail.RestockingAmount > 0 ? orderDetail.RestockingAmount * -1 : 0;
                        orderDetail.WarehouseTracking = GetReasonText(order.RGAReason) + (!string.IsNullOrEmpty(order.RGAReasonNotes) ? $" | {order.RGAReasonNotes}" : "");
                    }

                    creditMemo.CurrentCost = creditMemo.OrderDetails.Sum(e => e.UnitCost);
                    creditMemo.SubTotalAmount = creditMemo.OrderDetails.Sum(e => e.WholesalePrice * e.OrderQuantity);
                    creditMemo.TotalAmount = creditMemo.OrderDetails.Sum(e => e.DiscountedPrice * e.OrderQuantity);
                    creditMemo.RestockingAmount = creditMemo.OrderDetails.Sum(e => e.RestockingAmount);


                    // If Tax Rate > 0 Compute for Tax Amount then add to Total Amount;
                    if (creditMemo.TaxRate > 0)
                    {
                        creditMemo.TotalTax = creditMemo.TotalAmount * creditMemo.TaxRate / 100;
                        creditMemo.TotalAmount += creditMemo.TotalTax;
                    }

                    if (creditMemo.RestockingAmount != 0)
                    {
                        creditMemo.TotalAmount -= creditMemo.RestockingAmount;
                    }

                    creditMemo.Balance = creditMemo.TotalAmount;

                    #endregion
                    result = await _orderRepository.CreateCreditMemo(creditMemo);

                    if (result)
                    {
                        #region Prepare Update IsCreditMemoCreated
                        foreach (var orderDetail in creditMemo.OrderDetails)
                        {
                            var od = order.OrderDetails.Where(e => e.ProductId == orderDetail.ProductId).FirstOrDefault();

                            if (od != null)
                            {
                                od.IsCreditMemoCreated = true;
                            }
                        }

                        result = await _orderRepository.Update(order);
                        #endregion

                        // Email Credit Memo
                        //var orderDetails = await _dataContext.OrderDetails.Where(e => e.OrderId == creditMemo.Id && e.IsActive && !e.IsDeleted).ToListAsync();
                        var customer = await _dataContext.Customers.FirstOrDefaultAsync(e => e.Id == creditMemo.CustomerId);
                        if (customer != null)
                        {
                            var contacts = await _contactRepository.GetContactsByCustomerId(creditMemo.CustomerId);
                            if (contacts != null)
                            {
                                var contactList = contacts.Where(e => e.IsEmailCreditMemo).ToList();

                                if (contactList.Any())
                                {
                                    await _emailHelper.SendOrderEmailByContacts(creditMemo, creditMemo.OrderDetails, customer, contactList);
                                    return Ok(result);
                                }

                                //return BadRequest("Contacts not found!");
                            }
                            //else return BadRequest("Contacts not found!");
                        }
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GetReasonText(int rgaReason)
        {
            switch (rgaReason)
            {
                case 1: return "Damaged";
                case 2: return "Agent Sold Wrong";
                case 3: return "Car Totaled";
                case 4: return "Defective";
                case 5: return "Customer Do Not Need";
                case 6: return "Price and Billing Adjustment";
                case 7: return "Manager Approval";
                case 8: return "Price Too High";
                case 9: return "Got Somewhere Else";
                case 10: return "Customer Ordered Wrong";
                default: return string.Empty;
            }
        }

        [HttpPut("UpdateOrderSummary")]
        public async Task<ActionResult<bool>> UpdateOrderSummary(Order order)
        {
            try
            {
                var result = await _orderRepository.UpdateOrderSummary(order);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteOrder")]
        public async Task<ActionResult<List<Order>>> DeleteOrder(List<int> orderIds)
        {
            try
            {
                var orderList = await _orderRepository.Delete(orderIds);
                return Ok(orderList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteOrderDetail")]
        public async Task<ActionResult<bool>> DeleteOrderDetail([FromQuery] int orderDetailId)
        {
            try
            {
                var result = await _orderRepository.DeleteOrderDetail(orderDetailId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
