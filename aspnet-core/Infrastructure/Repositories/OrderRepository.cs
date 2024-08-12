using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.DTO.Paginated;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;
using Service.Email;
using System.Data;

namespace Infrastucture.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _context;
        private readonly IEmailService _emailService;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public OrderRepository(DataContext context, IEmailService emailService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _emailService = emailService;
        }

        #region Get Data
        public async Task<List<Order>> GetOrders()
        {
            var result = await _context.Orders.Where(e => !e.IsQuote && e.IsActive && !e.IsDeleted).ToListAsync();
            foreach (var order in result)
            {
                order.Customer = await _context.Customers.Where(e => e.Id == order.CustomerId).FirstOrDefaultAsync();
                order.CustomerName = order.Customer != null ? order.Customer.CustomerName : "";
                order.AccountNumber = order.Customer != null ? order.Customer.AccountNumber : 0;

                var priceLevel = new PriceLevel();

                if (order.Customer != null)
                {
                    priceLevel = await _context.PriceLevels.Where(e => e.Id == order.Customer.PriceLevelId).FirstOrDefaultAsync();
                }

                order.PriceLevelName = priceLevel != null ? priceLevel.LevelName : "";

                order.PaymentTerm = await _context.PaymentTerms.Where(e => e.Id == order.PaymentTermId).FirstOrDefaultAsync();
                order.PaymentTermName = order.PaymentTerm != null ? order.PaymentTerm.TermName : "";
                
                order.Warehouse = await _context.Warehouses.Where(e => e.Id == order.WarehouseId).FirstOrDefaultAsync();
                order.WarehouseName = order.Warehouse != null ? order.Warehouse.WarehouseName : "";

                order.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).OrderBy(od => od.PartNumber).ToListAsync();

                foreach (var detail in order.OrderDetails)
                {
                    detail.VendorCatalogs = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == detail.MainPartsLinkNumber.Trim()).ToListAsync();
                }
            }

            return result;
        }

        public async Task<Order?> GetOrderByOrderNumber(int orderNumber)
        {
            var result = await _context.Orders.FirstOrDefaultAsync(e => e.OrderNumber == orderNumber && e.IsActive && !e.IsDeleted && !e.IsQuote && (e.OrderStatusId != 3 && e.OrderStatusId != 4));
            
            if (result != null)
            {
                result.OrderDetails = new List<OrderDetail>();
                var orderDetails = await _context.OrderDetails.Where(e => e.OrderId == result.Id && e.IsActive && !e.IsDeleted && e.OrderQuantity > 0 && (e.StatusId == null || e.StatusId == 7))
                    .OrderBy(od => od.PartNumber).ToListAsync();
                if (orderDetails != null)
                {
                    result.OrderDetails = orderDetails.ToList();
                }

                //// Check of Order Status is Posted
                //if (result.OrderStatusId == 2)
                //{
                //    var orderDetails = await _context.OrderDetails.Where(e => e.OrderId == result.Id && e.IsActive && !e.IsDeleted && e.OrderQuantity > 0).OrderBy(od => od.PartNumber).ToListAsync();
                //    if (orderDetails != null)
                //    {
                //        result.OrderDetails = orderDetails.ToList();
                //    }
                //}
                //else
                //{
                //    //var orderDetails = await _context.OrderDetails.Where(e => e.OrderId == result.Id && e.IsActive && !e.IsDeleted && e.OrderQuantity > 0 && (e.StatusId == null || e.StatusId == 6))
                //    var orderDetails = await _context.OrderDetails.Where(e => e.OrderId == result.Id && e.IsActive && !e.IsDeleted && e.OrderQuantity > 0 && e.StatusId == null)
                //        .OrderBy(od => od.PartNumber).ToListAsync();
                //    if (orderDetails == null)
                //        result.OrderDetails = new List<OrderDetail>();
                //    else
                //        result.OrderDetails = orderDetails.ToList();
                //}
            }

            return result;
        }

        public async Task<List<Order>> GetOrdersByCustomerId(int customerId)
        {
            var result = await _context.Orders.Where(e => e.CustomerId == customerId && !e.IsQuote && e.IsActive && !e.IsDeleted).ToListAsync();
            
            foreach (var order in result)
            {
                order.Customer = await _context.Customers.Where(e => e.Id == order.CustomerId).FirstOrDefaultAsync();
                order.CustomerName = order.Customer != null ? order.Customer.CustomerName : "";
                order.AccountNumber = order.Customer != null ? order.Customer.AccountNumber : 0;

                var priceLevel = new PriceLevel();

                if (order.Customer != null)
                {
                    priceLevel = await _context.PriceLevels.Where(e => e.Id == order.Customer.PriceLevelId).FirstOrDefaultAsync();
                }

                order.PriceLevelName = priceLevel != null ? priceLevel.LevelName : "";

                order.PaymentTerm = await _context.PaymentTerms.Where(e => e.Id == order.PaymentTermId).FirstOrDefaultAsync();
                order.PaymentTermName = order.PaymentTerm != null ? order.PaymentTerm.TermName : "";

                order.Warehouse = await _context.Warehouses.Where(e => e.Id == order.WarehouseId).FirstOrDefaultAsync();
                order.WarehouseName = order.Warehouse != null ? order.Warehouse.WarehouseName : "";

                order.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).OrderBy(od => od.PartNumber).ToListAsync();

                foreach (var detail in order.OrderDetails)
                {
                    detail.VendorCatalogs = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == detail.MainPartsLinkNumber.Trim()).ToListAsync();
                }
            }

            return result;
        }

        public async Task<List<Order>> GetInvoicesByCustomerId(int customerId)
        {
            var result = await _context.Orders.Where(e => e.CustomerId == customerId && !e.IsQuote && e.IsActive && !e.IsDeleted && 
                (e.OrderStatusId == 1 || e.OrderStatusId == 4 || e.PaymentReference == "Partial" ||
                 (e.OrderStatusId == 2 && e.Balance > 0))).ToListAsync();

            foreach (var order in result)
            {
                order.Customer = await _context.Customers.Where(e => e.Id == order.CustomerId).FirstOrDefaultAsync();
                order.CustomerName = order.Customer != null ? order.Customer.CustomerName : "";
                order.AccountNumber = order.Customer != null ? order.Customer.AccountNumber : 0;

                var priceLevel = new PriceLevel();

                if (order.Customer != null)
                {
                    priceLevel = await _context.PriceLevels.Where(e => e.Id == order.Customer.PriceLevelId).FirstOrDefaultAsync();
                }

                order.PriceLevelName = priceLevel != null ? priceLevel.LevelName : "";

                order.PaymentTerm = await _context.PaymentTerms.Where(e => e.Id == order.PaymentTermId).FirstOrDefaultAsync();
                order.PaymentTermName = order.PaymentTerm != null ? order.PaymentTerm.TermName : "";

                order.Warehouse = await _context.Warehouses.Where(e => e.Id == order.WarehouseId).FirstOrDefaultAsync();
                order.WarehouseName = order.Warehouse != null ? order.Warehouse.WarehouseName : "";

                order.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).OrderBy(od => od.PartNumber).ToListAsync();

                foreach (var detail in order.OrderDetails)
                {
                    detail.VendorCatalogs = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == detail.MainPartsLinkNumber.Trim()).ToListAsync();
                }
            }

            return result;
        }

        public async Task<List<Order>> GetInvoicesByCustomerIds(List<int> customerIds)
        {
            var result = await _context.Orders.Where(e => customerIds.Contains(e.CustomerId) && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                (e.OrderStatusId == 1 || e.OrderStatusId == 4 || e.PaymentReference == "Partial" ||
                 (e.OrderStatusId == 2 && e.Balance > 0))).OrderBy(o => o.CustomerId).ThenBy(i => i.OrderNumber).ToListAsync();

            foreach (var order in result)
            {
                order.Customer = await _context.Customers.Where(e => e.Id == order.CustomerId).FirstOrDefaultAsync();
                order.CustomerName = order.Customer != null ? order.Customer.CustomerName : "";
                order.AccountNumber = order.Customer != null ? order.Customer.AccountNumber : 0;

                var priceLevel = new PriceLevel();

                if (order.Customer != null)
                {
                    priceLevel = await _context.PriceLevels.Where(e => e.Id == order.Customer.PriceLevelId).FirstOrDefaultAsync();
                }

                order.PriceLevelName = priceLevel != null ? priceLevel.LevelName : "";

                order.PaymentTerm = await _context.PaymentTerms.Where(e => e.Id == order.PaymentTermId).FirstOrDefaultAsync();
                order.PaymentTermName = order.PaymentTerm != null ? order.PaymentTerm.TermName : "";

                order.Warehouse = await _context.Warehouses.Where(e => e.Id == order.WarehouseId).FirstOrDefaultAsync();
                order.WarehouseName = order.Warehouse != null ? order.Warehouse.WarehouseName : "";

                order.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).OrderBy(od => od.PartNumber).ToListAsync();

                foreach (var detail in order.OrderDetails)
                {
                    detail.VendorCatalogs = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == detail.MainPartsLinkNumber.Trim()).ToListAsync();
                }
            }

            return result;
        }

        public async Task<List<Order>> GetCreditMemoByInvoiceNumber(int invoiceNumber)
        {
            var result = await _context.Orders.Where(e => e.OriginalInvoiceNumber == invoiceNumber && e.IsActive && !e.IsDeleted).ToListAsync();

            foreach (var order in result)
            {
                order.Customer = await _context.Customers.Where(e => e.Id == order.CustomerId).FirstOrDefaultAsync();
                order.CustomerName = order.Customer != null ? order.Customer.CustomerName : "";
                order.AccountNumber = order.Customer != null ? order.Customer.AccountNumber : 0;

                var priceLevel = new PriceLevel();

                if (order.Customer != null)
                {
                    priceLevel = await _context.PriceLevels.Where(e => e.Id == order.Customer.PriceLevelId).FirstOrDefaultAsync();
                }

                order.PriceLevelName = priceLevel != null ? priceLevel.LevelName : "";

                order.PaymentTerm = await _context.PaymentTerms.Where(e => e.Id == order.PaymentTermId).FirstOrDefaultAsync();
                order.PaymentTermName = order.PaymentTerm != null ? order.PaymentTerm.TermName : "";

                order.Warehouse = await _context.Warehouses.Where(e => e.Id == order.WarehouseId).FirstOrDefaultAsync();
                order.WarehouseName = order.Warehouse != null ? order.Warehouse.WarehouseName : "";

                order.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).OrderBy(od => od.PartNumber).ToListAsync();

                foreach (var detail in order.OrderDetails)
                {
                    detail.VendorCatalogs = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == detail.MainPartsLinkNumber.Trim()).ToListAsync();
                }
            }

            return result;
        }

        public async Task<List<Order>> GetCreditMemoByCustomerId(int customerId)
        {
            var result = await _context.Orders.Where(e => (e.OrderStatusId == 5 || e.OrderStatusId == 7) && e.CustomerId == customerId && e.Balance < 0 && e.IsActive && !e.IsDeleted).ToListAsync();
            
            if (result != null)
            {
                foreach (var order in result)
                {
                    order.Customer = await _context.Customers.Where(e => e.Id == order.CustomerId).FirstOrDefaultAsync();
                    order.CustomerName = order.Customer != null ? order.Customer.CustomerName : "";
                    order.AccountNumber = order.Customer != null ? order.Customer.AccountNumber : 0;

                    var priceLevel = new PriceLevel();

                    if (order.Customer != null)
                    {
                        priceLevel = await _context.PriceLevels.Where(e => e.Id == order.Customer.PriceLevelId).FirstOrDefaultAsync();
                    }

                    order.PriceLevelName = priceLevel != null ? priceLevel.LevelName : "";

                    order.PaymentTerm = await _context.PaymentTerms.Where(e => e.Id == order.PaymentTermId).FirstOrDefaultAsync();
                    order.PaymentTermName = order.PaymentTerm != null ? order.PaymentTerm.TermName : "";

                    order.Warehouse = await _context.Warehouses.Where(e => e.Id == order.WarehouseId).FirstOrDefaultAsync();
                    order.WarehouseName = order.Warehouse != null ? order.Warehouse.WarehouseName : "";

                    order.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).OrderBy(od => od.PartNumber).ToListAsync();

                    foreach (var detail in order.OrderDetails)
                    {
                        detail.VendorCatalogs = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == detail.MainPartsLinkNumber.Trim()).ToListAsync();
                    }
                }

                return result;
            }

            return new List<Order>();
        }

        public async Task<List<Order>> GetCreditMemoByCustomerIds(List<int> customerIds)
        {
            var result = await _context.Orders.Where(e => (e.OrderStatusId == 5 || e.OrderStatusId == 7) && customerIds.Contains(e.CustomerId) && e.Balance < 0 && e.IsActive && !e.IsDeleted)
                .OrderBy(o => o.CustomerId).ThenBy(i => i.OrderNumber).ToListAsync();

            if (result != null)
            {
                foreach (var order in result)
                {
                    order.Customer = await _context.Customers.Where(e => e.Id == order.CustomerId).FirstOrDefaultAsync();
                    order.CustomerName = order.Customer != null ? order.Customer.CustomerName : "";
                    order.AccountNumber = order.Customer != null ? order.Customer.AccountNumber : 0;

                    var priceLevel = new PriceLevel();

                    if (order.Customer != null)
                    {
                        priceLevel = await _context.PriceLevels.Where(e => e.Id == order.Customer.PriceLevelId).FirstOrDefaultAsync();
                    }

                    order.PriceLevelName = priceLevel != null ? priceLevel.LevelName : "";

                    order.PaymentTerm = await _context.PaymentTerms.Where(e => e.Id == order.PaymentTermId).FirstOrDefaultAsync();
                    order.PaymentTermName = order.PaymentTerm != null ? order.PaymentTerm.TermName : "";

                    order.Warehouse = await _context.Warehouses.Where(e => e.Id == order.WarehouseId).FirstOrDefaultAsync();
                    order.WarehouseName = order.Warehouse != null ? order.Warehouse.WarehouseName : "";

                    order.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).OrderBy(od => od.PartNumber).ToListAsync();

                    foreach (var detail in order.OrderDetails)
                    {
                        detail.VendorCatalogs = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == detail.MainPartsLinkNumber.Trim()).ToListAsync();
                    }
                }

                return result;
            }

            return new List<Order>();
        }

        public async Task<List<Order>> GetQuotes()
        {
            var result = await _context.Orders.Where(e => e.IsQuote == true && e.IsActive && !e.IsDeleted).ToListAsync();
            foreach (var order in result)
            {
                order.Customer = await _context.Customers.Where(e => e.Id == order.CustomerId).FirstOrDefaultAsync();
                order.CustomerName = order.Customer != null ? order.Customer.CustomerName : "";
                order.AccountNumber = order.Customer != null ? order.Customer.AccountNumber : 0;

                var priceLevel = new PriceLevel();

                if (order.Customer != null)
                {
                    priceLevel = await _context.PriceLevels.Where(e => e.Id == order.Customer.PriceLevelId).FirstOrDefaultAsync();
                }

                order.PriceLevelName = priceLevel != null ? priceLevel.LevelName : "";

                order.PaymentTerm = await _context.PaymentTerms.Where(e => e.Id == order.PaymentTermId).FirstOrDefaultAsync();
                order.PaymentTermName = order.PaymentTerm != null ? order.PaymentTerm.TermName : "";

                order.Warehouse = await _context.Warehouses.Where(e => e.Id == order.WarehouseId).FirstOrDefaultAsync();
                order.WarehouseName = order.Warehouse != null ? order.Warehouse.WarehouseName : "";

                order.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).OrderBy(od => od.PartNumber).ToListAsync();

                foreach (var detail in order.OrderDetails)
                {
                    detail.VendorCatalogs = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == detail.MainPartsLinkNumber.Trim()).ToListAsync();
                }
            }

            return result;
        }

        public async Task<PaginatedListDTO<Order>> GetOrdersPaginated(int searchType, int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search, int? paymentTerm)
        {
            var result = new PaginatedListDTO<Order>()
            {
                Data = new List<Order>(),
                RecordCount = 0
            };

            List<Order> orders = new List<Order>();
            int recordCount = 0;

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                switch (searchType)
                {
                    case 1: // Customer Name
                        { 
                            recordCount = 
                                (from order in _context.Orders
                                 join customer in _context.Customers on order.CustomerId equals customer.Id
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && 
                                 (!string.IsNullOrEmpty(customer.CustomerName) ? customer.CustomerName.Trim().ToLower().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await 
                                    (from order in _context.Orders
                                     join customer in _context.Customers on order.CustomerId equals customer.Id
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 &&
                                     (!string.IsNullOrEmpty(customer.CustomerName) ? customer.CustomerName.Trim().ToLower().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break; 
                        }
                    case 2: // Phone Number
                        {
                            recordCount =
                                (from order in _context.Orders
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && 
                                 (!string.IsNullOrEmpty(order.PhoneNumber) ? order.PhoneNumber.Trim().ToLower().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 &&
                                     (!string.IsNullOrEmpty(order.PhoneNumber) ? order.PhoneNumber.Trim().ToLower().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 3: // Order Number
                        {
                            recordCount =
                                (from order in _context.Orders
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && 
                                 (order.OrderNumber != null ? order.OrderNumber.Value.ToString().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && 
                                     (order.OrderNumber != null ? order.OrderNumber.Value.ToString().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 4: // Purchase Order Number
                        {
                            recordCount =
                                (from order in _context.Orders
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 &&
                                 (!string.IsNullOrEmpty(order.PurchaseOrderNumber) ? order.PurchaseOrderNumber.Trim().ToLower().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 &&
                                     (!string.IsNullOrEmpty(order.PurchaseOrderNumber) ? order.PurchaseOrderNumber.Trim().ToLower().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 5: // Part Number
                        {
                            recordCount =
                                (from order in _context.Orders
                                 join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && orderDetail.IsActive && !orderDetail.IsDeleted &&
                                 (!string.IsNullOrEmpty(orderDetail.PartNumber) ? orderDetail.PartNumber.Trim().ToLower().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && orderDetail.IsActive && !orderDetail.IsDeleted &&
                                     (!string.IsNullOrEmpty(orderDetail.PartNumber) ? orderDetail.PartNumber.Trim().ToLower().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 6: // Main Parts Link Number
                        {
                            recordCount =
                                (from order in _context.Orders
                                 join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && orderDetail.IsActive && !orderDetail.IsDeleted &&
                                 (!string.IsNullOrEmpty(orderDetail.MainPartsLinkNumber) ? orderDetail.MainPartsLinkNumber.Trim().ToLower().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && orderDetail.IsActive && !orderDetail.IsDeleted &&
                                     (!string.IsNullOrEmpty(orderDetail.MainPartsLinkNumber) ? orderDetail.MainPartsLinkNumber.Trim().ToLower().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 7: // Main OEM Number
                        {
                            recordCount =
                                (from order in _context.Orders
                                 join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && orderDetail.IsActive && !orderDetail.IsDeleted &&
                                 (!string.IsNullOrEmpty(orderDetail.MainOEMNumber) ? orderDetail.MainOEMNumber.Trim().ToLower().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && orderDetail.IsActive && !orderDetail.IsDeleted &&
                                     (!string.IsNullOrEmpty(orderDetail.MainOEMNumber) ? orderDetail.MainOEMNumber.Trim().ToLower().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 8: // Address
                        {
                            recordCount =
                                (from order in _context.Orders
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 &&
                                 (!string.IsNullOrEmpty(order.ShipAddress) ? order.ShipAddress.Trim().ToLower().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 &&
                                     (!string.IsNullOrEmpty(order.ShipAddress) ? order.ShipAddress.Trim().ToLower().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 9: // Created By
                        {
                            recordCount =
                                (from order in _context.Orders
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 &&
                                 (!string.IsNullOrEmpty(order.CreatedBy) ? order.CreatedBy.Trim().ToLower().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 &&
                                     (!string.IsNullOrEmpty(order.CreatedBy) ? order.CreatedBy.Trim().ToLower().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 10: // Invoice Number
                        {
                            recordCount =
                                (from order in _context.Orders
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 &&
                                 (order.InvoiceNumber != null ? order.InvoiceNumber.Value.ToString().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 &&
                                     (order.InvoiceNumber != null ? order.InvoiceNumber.Value.ToString().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 11: // Account Number
                        {
                            recordCount =
                                (from order in _context.Orders
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && order.AccountNumber.ToString().Contains(search)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && order.AccountNumber.ToString().Contains(search)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 12: // Order Status
                        {
                            recordCount =
                                (from order in _context.Orders
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 &&
                                 (!string.IsNullOrEmpty(order.OrderStatusName) ? order.OrderStatusName.Trim().ToLower().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 &&
                                     (!string.IsNullOrEmpty(order.OrderStatusName) ? order.OrderStatusName.Trim().ToLower().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 13: // Payment Term
                        {
                            var filter = int.Parse(search);
                            recordCount =
                                (from order in _context.Orders
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && 
                                 (order.PaymentTerm != null ? order.PaymentTermId == paymentTerm : false) && 
                                 (filter == 1 ? order.Balance == 0 : filter == 2 ? order.Balance < 0 : order.Balance > 0)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 &&
                                     (order.PaymentTerm != null ? order.PaymentTermId == paymentTerm : false) &&
                                     (filter == 1 ? order.Balance == 0 : filter == 2 ? order.Balance < 0 : order.Balance > 0)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    default:
                        {
                            recordCount = 
                                (from order in _context.Orders
                                 join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                                 join customer in _context.Customers on order.CustomerId equals customer.Id
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (
                                     (!string.IsNullOrEmpty(customer.CustomerName) ? customer.CustomerName.Trim().ToLower().Contains(search) : false) ||
                                     (!string.IsNullOrEmpty(order.PhoneNumber) ? order.PhoneNumber.Trim().ToLower().Contains(search) : false) ||
                                     (order.OrderNumber != null ? order.OrderNumber.Value.ToString().Contains(search) : false) ||
                                     (!string.IsNullOrEmpty(order.PurchaseOrderNumber) ? order.PurchaseOrderNumber.Trim().ToLower().Contains(search) : false) ||
                                     (!string.IsNullOrEmpty(orderDetail.PartNumber) ? orderDetail.PartNumber.Trim().ToLower().Contains(search) : false) ||
                                     (!string.IsNullOrEmpty(orderDetail.MainPartsLinkNumber) ? orderDetail.MainPartsLinkNumber.Trim().ToLower().Contains(search) : false) ||
                                     (!string.IsNullOrEmpty(orderDetail.MainOEMNumber) ? orderDetail.MainOEMNumber.Trim().ToLower().Contains(search) : false) ||
                                     (!string.IsNullOrEmpty(order.ShipAddress) ? order.ShipAddress.Trim().ToLower().Contains(search) : false) ||
                                     (!string.IsNullOrEmpty(order.CreatedBy) ? order.CreatedBy.Trim().ToLower().Contains(search) : false) ||
                                     (order.InvoiceNumber != null ? order.InvoiceNumber.Value.ToString().Contains(search) : false) ||
                                     order.AccountNumber.ToString().Contains(search) ||
                                     (!string.IsNullOrEmpty(order.OrderStatusName) ? order.OrderStatusName.Trim().ToLower().Contains(search) : false))
                                 select order)
                                 .Distinct()
                                 .Count();

                            if (recordCount > 0)
                            {
                                orders = await (
                                    from order in _context.Orders
                                    join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                                    join customer in _context.Customers on order.CustomerId equals customer.Id
                                    where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (
                                         (!string.IsNullOrEmpty(customer.CustomerName) ? customer.CustomerName.Trim().ToLower().Contains(search) : false) ||
                                         (!string.IsNullOrEmpty(order.PhoneNumber) ? order.PhoneNumber.Trim().ToLower().Contains(search) : false) ||
                                         (order.OrderNumber != null ? order.OrderNumber.Value.ToString().Contains(search) : false) ||
                                         (!string.IsNullOrEmpty(order.PurchaseOrderNumber) ? order.PurchaseOrderNumber.Trim().ToLower().Contains(search) : false) ||
                                         (!string.IsNullOrEmpty(orderDetail.PartNumber) ? orderDetail.PartNumber.Trim().ToLower().Contains(search) : false) ||
                                         (!string.IsNullOrEmpty(orderDetail.MainPartsLinkNumber) ? orderDetail.MainPartsLinkNumber.Trim().ToLower().Contains(search) : false) ||
                                         (!string.IsNullOrEmpty(orderDetail.MainOEMNumber) ? orderDetail.MainOEMNumber.Trim().ToLower().Contains(search) : false) ||
                                         (!string.IsNullOrEmpty(order.ShipAddress) ? order.ShipAddress.Trim().ToLower().Contains(search) : false) ||
                                         (!string.IsNullOrEmpty(order.CreatedBy) ? order.CreatedBy.Trim().ToLower().Contains(search) : false) ||
                                         (order.InvoiceNumber != null ? order.InvoiceNumber.Value.ToString().Contains(search) : false) ||
                                         order.AccountNumber.ToString().Contains(search) ||
                                         (!string.IsNullOrEmpty(order.OrderStatusName) ? order.OrderStatusName.Trim().ToLower().Contains(search) : false))
                                    select order)
                                    .Distinct()
                                    .OrderByDescending(e => e.OrderNumber)
                                    .Skip(pageSize * pageIndex)
                                    .Take(pageSize)
                                    .ToListAsync();
                            }
                            break;
                        }
                }
            }
            else // search is null or empty
            {
                recordCount =
                    (from order in _context.Orders
                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9
                     select order)
                    .Distinct()
                    .Count();

                if (recordCount > 0)
                {
                    orders = await (
                    from order in _context.Orders
                    where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9
                    select order)
                    .Distinct()
                    .OrderByDescending(e => e.OrderNumber)
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync();
                }
            }
            
            foreach (var order in orders)
            {
                order.Customer = await _context.Customers.Where(e => e.Id == order.CustomerId).FirstOrDefaultAsync();
                order.CustomerName = order.Customer != null ? order.Customer.CustomerName : "";
                order.AccountNumber = order.Customer != null ? order.Customer.AccountNumber : 0;

                var priceLevel = new PriceLevel();
                
                if (order.Customer != null)
                {
                    priceLevel = await _context.PriceLevels.Where(e => e.Id == order.Customer.PriceLevelId).FirstOrDefaultAsync();
                }
                
                order.PriceLevelName = priceLevel != null ? priceLevel.LevelName : "";

                order.PaymentTerm = await _context.PaymentTerms.Where(e => e.Id == order.PaymentTermId).FirstOrDefaultAsync();
                order.PaymentTermName = order.PaymentTerm != null ? order.PaymentTerm.TermName : "";

                order.Warehouse = await _context.Warehouses.Where(e => e.Id == order.WarehouseId).FirstOrDefaultAsync();
                order.WarehouseName = order.Warehouse != null ? order.Warehouse.WarehouseName : "";

                order.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).OrderBy(od => od.PartNumber).ToListAsync();

                foreach (var detail in order.OrderDetails)
                {
                    detail.VendorCatalogs = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == detail.MainPartsLinkNumber.Trim()).OrderBy(vc => vc.VendorCode).ToListAsync();

                    //var venCat = await (
                    //    from vc in _context.VendorCatalogs
                    //    join v in _context.Vendors on vc.VendorCode.Trim().ToLower() equals v.VendorCode.Trim().ToLower()
                    //    where vc.PartsLinkNumber == detail.MainPartsLinkNumber && 
                    //    (v.IsCAVendor == (order.BillState == "CA") || v.IsNVVendor == (order.BillState == "NV"))
                        

                    //    select vc)
                    //    .Distinct()
                    //    .OrderBy(e => e.VendorCode)
                    //    .ToListAsync();

                    //detail.VendorCatalogs = venCat; 

                    //Get Warehouse Stocks with Location Name
                    var stocks = (
                        from stock in _context.WarehouseStocks
                        join product in _context.Products on stock.ProductId equals product.Id
                        join location in _context.WarehouseLocations on stock.WarehouseLocationId equals location.Id
                        //where stock.ProductId == detail.ProductId && (order.Customer.State == "NV" ? true : stock.WarehouseId == 1)
                        where stock.ProductId == detail.ProductId
                        select new WarehouseStock
                        {
                            Id = stock.Id,
                            CreatedBy = stock.CreatedBy,
                            CreatedDate = stock.CreatedDate,
                            IsActive = stock.IsActive,
                            IsDeleted = stock.IsDeleted,
                            Location = location.Location,
                            ModifiedBy = stock.ModifiedBy,
                            ModifiedDate = stock.ModifiedDate,
                            ProductId = detail.ProductId,
                            Quantity = stock.Quantity > 0 ? stock.Quantity : 0,
                            WarehouseId = stock.WarehouseId,
                            WarehouseLocationId = stock.WarehouseLocationId,
                            CurrentCost = product.CurrentCost.HasValue ? product.CurrentCost.Value : 0
                        }).ToList();

                    detail.Stocks = stocks;

                    var warehouseTrackings = await _context.WarehouseTrackings.Where(e => e.OrderDetailId == detail.Id && e.IsActive).OrderByDescending(e => e.Id).ToListAsync();
                    detail.WarehouseTrackings = warehouseTrackings;
                }
            }

            if (recordCount > 0)
            {
                result.Data = orders;
                result.RecordCount = recordCount;
            }
            
            return result;
        }

        public async Task<PaginatedListDTO<Order>> GetOrdersByDatePaginated(int searchType, int pageSize, int pageIndex, DateTime fromDate, DateTime toDate, string? search, int? paymentTerm)
        {
            var result = new PaginatedListDTO<Order>()
            {
                Data = new List<Order>(),
                RecordCount = 0
            };

            List<Order> orders = new List<Order>();
            int recordCount = 0;

            // US Settings
            DateTime fDate = fromDate; // new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, fromDate.Hour, fromDate.Minute, fromDate.Second);
            DateTime tDate = toDate.AddDays(1); // new DateTime(toDate.Year, toDate.Month, toDate.Day + 1, toDate.Hour, toDate.Minute, toDate.Second);

            //// PH Settings
            //DateTime fDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day - 1, 16, 00, 00);
            //DateTime tDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 16, 00, 00);

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                switch (searchType)
                {
                    case 1: // Customer Name
                        {
                            recordCount =
                                (from order in _context.Orders
                                 join customer in _context.Customers on order.CustomerId equals customer.Id
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) &&
                                 (!string.IsNullOrEmpty(customer.CustomerName) ? customer.CustomerName.Trim().ToLower().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     join customer in _context.Customers on order.CustomerId equals customer.Id
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) &&
                                     (!string.IsNullOrEmpty(customer.CustomerName) ? customer.CustomerName.Trim().ToLower().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 2: // Phone Number
                        {
                            recordCount =
                                (from order in _context.Orders
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) &&
                                 (!string.IsNullOrEmpty(order.PhoneNumber) ? order.PhoneNumber.Trim().ToLower().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) &&
                                     (!string.IsNullOrEmpty(order.PhoneNumber) ? order.PhoneNumber.Trim().ToLower().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 3: // Order Number
                        {
                            recordCount =
                                (from order in _context.Orders
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) &&
                                 (order.OrderNumber != null ? order.OrderNumber.Value.ToString().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) &&
                                     (order.OrderNumber != null ? order.OrderNumber.Value.ToString().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 4: // Purchase Order Number
                        {
                            recordCount =
                                (from order in _context.Orders
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) &&
                                 (!string.IsNullOrEmpty(order.PurchaseOrderNumber) ? order.PurchaseOrderNumber.Trim().ToLower().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) &&
                                     (!string.IsNullOrEmpty(order.PurchaseOrderNumber) ? order.PurchaseOrderNumber.Trim().ToLower().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 5: // Part Number
                        {
                            recordCount =
                                (from order in _context.Orders
                                 join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) && orderDetail.IsActive && !orderDetail.IsDeleted &&
                                 (!string.IsNullOrEmpty(orderDetail.PartNumber) ? orderDetail.PartNumber.Trim().ToLower().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) && orderDetail.IsActive && !orderDetail.IsDeleted &&
                                     (!string.IsNullOrEmpty(orderDetail.PartNumber) ? orderDetail.PartNumber.Trim().ToLower().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 6: // Main Parts Link Number
                        {
                            recordCount =
                                (from order in _context.Orders
                                 join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) && orderDetail.IsActive && !orderDetail.IsDeleted &&
                                 (!string.IsNullOrEmpty(orderDetail.MainPartsLinkNumber) ? orderDetail.MainPartsLinkNumber.Trim().ToLower().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) && orderDetail.IsActive && !orderDetail.IsDeleted &&
                                     (!string.IsNullOrEmpty(orderDetail.MainPartsLinkNumber) ? orderDetail.MainPartsLinkNumber.Trim().ToLower().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 7: // Main OEM Number
                        {
                            recordCount =
                                (from order in _context.Orders
                                 join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) && orderDetail.IsActive && !orderDetail.IsDeleted &&
                                 (!string.IsNullOrEmpty(orderDetail.MainOEMNumber) ? orderDetail.MainOEMNumber.Trim().ToLower().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) && orderDetail.IsActive && !orderDetail.IsDeleted &&
                                     (!string.IsNullOrEmpty(orderDetail.MainOEMNumber) ? orderDetail.MainOEMNumber.Trim().ToLower().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 8: // Address
                        {
                            recordCount =
                                (from order in _context.Orders
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) &&
                                 (!string.IsNullOrEmpty(order.ShipAddressName) ? order.ShipAddressName.Trim().ToLower().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) &&
                                     (!string.IsNullOrEmpty(order.ShipAddressName) ? order.ShipAddressName.Trim().ToLower().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 9: // Created By
                        {
                            recordCount =
                                (from order in _context.Orders
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) &&
                                 (!string.IsNullOrEmpty(order.CreatedBy) ? order.CreatedBy.Trim().ToLower().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) &&
                                     (!string.IsNullOrEmpty(order.CreatedBy) ? order.CreatedBy.Trim().ToLower().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 10: // Invoice Number
                        {
                            recordCount =
                                (from order in _context.Orders
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) &&
                                 (order.InvoiceNumber != null ? order.InvoiceNumber.Value.ToString().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) &&
                                     (order.InvoiceNumber != null ? order.InvoiceNumber.Value.ToString().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 11: // Account Number
                        {
                            recordCount =
                                (from order in _context.Orders
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) && order.AccountNumber.ToString().Contains(search)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) && order.AccountNumber.ToString().Contains(search)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 12: // Order Status
                        {
                            recordCount =
                                (from order in _context.Orders
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) &&
                                 (!string.IsNullOrEmpty(order.OrderStatusName) ? order.OrderStatusName.Trim().ToLower().Contains(search) : false)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) &&
                                     (!string.IsNullOrEmpty(order.OrderStatusName) ? order.OrderStatusName.Trim().ToLower().Contains(search) : false)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    case 13: // Payment Term
                        {
                            var filter = int.Parse(search);
                            recordCount =
                                (from order in _context.Orders
                                 where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) &&
                                 (order.PaymentTerm != null ? order.PaymentTermId == paymentTerm : false) &&
                                 (filter == 1 ? order.Balance == 0 : filter == 2 ? order.Balance < 0 : order.Balance > 0)
                                 select order)
                            .Distinct()
                            .Count();

                            if (recordCount > 0)
                            {
                                orders = await
                                    (from order in _context.Orders
                                     where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) &&
                                     (order.PaymentTerm != null ? order.PaymentTermId == paymentTerm : false) &&
                                     (filter == 1 ? order.Balance == 0 : filter == 2 ? order.Balance < 0 : order.Balance > 0)
                                     select order)
                                .Distinct()
                                .OrderByDescending(e => e.OrderNumber)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();
                            }
                            break;
                        }
                    default:
                        {
                            recordCount = (
                                from order in _context.Orders
                                join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                                join customer in _context.Customers on order.CustomerId equals customer.Id
                                where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) && (
                                    (!string.IsNullOrEmpty(customer.CustomerName) ? customer.CustomerName.Trim().ToLower().Contains(search) : false) ||
                                    (!string.IsNullOrEmpty(order.PhoneNumber) ? order.PhoneNumber.Trim().ToLower().Contains(search) : false) ||
                                    (order.OrderNumber != null ? order.OrderNumber.Value.ToString().Contains(search) : false) ||
                                    (!string.IsNullOrEmpty(order.PurchaseOrderNumber) ? order.PurchaseOrderNumber.Trim().ToLower().Contains(search) : false) ||
                                    (!string.IsNullOrEmpty(orderDetail.PartNumber) ? orderDetail.PartNumber.Trim().ToLower().Contains(search) : false) ||
                                    (!string.IsNullOrEmpty(orderDetail.MainPartsLinkNumber) ? orderDetail.MainPartsLinkNumber.Trim().ToLower().Contains(search) : false) ||
                                    (!string.IsNullOrEmpty(orderDetail.MainOEMNumber) ? orderDetail.MainOEMNumber.Trim().ToLower().Contains(search) : false) ||
                                    (!string.IsNullOrEmpty(order.ShipAddressName) ? order.ShipAddressName.Trim().ToLower().Contains(search) : false) ||
                                    (!string.IsNullOrEmpty(order.CreatedBy) ? order.CreatedBy.Trim().ToLower().Contains(search) : false) ||
                                    (order.InvoiceNumber != null ? order.InvoiceNumber.Value.ToString().Contains(search) : false) ||
                                    order.AccountNumber.ToString().Contains(search) ||
                                    (!string.IsNullOrEmpty(order.OrderStatusName) ? order.OrderStatusName.Trim().ToLower().Contains(search) : false))
                                 select order)
                                 .Distinct()
                                 .Count();

                            if (recordCount > 0)
                            {
                                orders = await (
                                    from order in _context.Orders
                                    join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                                    join customer in _context.Customers on order.CustomerId equals customer.Id
                                    where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate) && (
                                        (!string.IsNullOrEmpty(customer.CustomerName) ? customer.CustomerName.Trim().ToLower().Contains(search) : false) ||
                                        (!string.IsNullOrEmpty(order.PhoneNumber) ? order.PhoneNumber.Trim().ToLower().Contains(search) : false) ||
                                        (order.OrderNumber != null ? order.OrderNumber.Value.ToString().Contains(search) : false) ||
                                        (!string.IsNullOrEmpty(order.PurchaseOrderNumber) ? order.PurchaseOrderNumber.Trim().ToLower().Contains(search) : false) ||
                                        (!string.IsNullOrEmpty(orderDetail.PartNumber) ? orderDetail.PartNumber.Trim().ToLower().Contains(search) : false) ||
                                        (!string.IsNullOrEmpty(orderDetail.MainPartsLinkNumber) ? orderDetail.MainPartsLinkNumber.Trim().ToLower().Contains(search) : false) ||
                                        (!string.IsNullOrEmpty(orderDetail.MainOEMNumber) ? orderDetail.MainOEMNumber.Trim().ToLower().Contains(search) : false) ||
                                        (!string.IsNullOrEmpty(order.ShipAddressName) ? order.ShipAddressName.Trim().ToLower().Contains(search) : false) ||
                                        (!string.IsNullOrEmpty(order.CreatedBy) ? order.CreatedBy.Trim().ToLower().Contains(search) : false) ||
                                        (order.InvoiceNumber != null ? order.InvoiceNumber.Value.ToString().Contains(search) : false) ||
                                        order.AccountNumber.ToString().Contains(search) ||
                                        (!string.IsNullOrEmpty(order.OrderStatusName) ? order.OrderStatusName.Trim().ToLower().Contains(search) : false))
                                    select order)
                                    .Distinct()
                                    .OrderByDescending(e => e.OrderNumber)
                                    .Skip(pageSize * pageIndex)
                                    .Take(pageSize)
                                    .ToListAsync();
                            }
                            break;
                        }
                }
            }
            else
            {
                recordCount = (
                from order in _context.Orders
                join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                join customer in _context.Customers on order.CustomerId equals customer.Id
                where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate)
                select order)
                .Distinct()
                .Count();

                if (recordCount > 0)
                {
                    orders = await (
                    from order in _context.Orders
                    join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                    join customer in _context.Customers on order.CustomerId equals customer.Id
                    where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId != 9 && (order.OrderDate >= fDate && order.OrderDate < tDate)
                    select order)
                    .Distinct()
                    .OrderByDescending(e => e.OrderNumber)
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync();
                }
            }

            foreach (var order in orders)
            {
                order.Customer = await _context.Customers.Where(e => e.Id == order.CustomerId).FirstOrDefaultAsync();
                order.CustomerName = order.Customer != null ? order.Customer.CustomerName : "";
                order.AccountNumber = order.Customer != null ? order.Customer.AccountNumber : 0;

                var priceLevel = new PriceLevel();

                if (order.Customer != null)
                {
                    priceLevel = await _context.PriceLevels.Where(e => e.Id == order.Customer.PriceLevelId).FirstOrDefaultAsync();
                }

                order.PriceLevelName = priceLevel != null ? priceLevel.LevelName : "";

                order.PaymentTerm = await _context.PaymentTerms.Where(e => e.Id == order.PaymentTermId).FirstOrDefaultAsync();
                order.PaymentTermName = order.PaymentTerm != null ? order.PaymentTerm.TermName : "";

                order.Warehouse = await _context.Warehouses.Where(e => e.Id == order.WarehouseId).FirstOrDefaultAsync();
                order.WarehouseName = order.Warehouse != null ? order.Warehouse.WarehouseName : "";

                order.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).OrderBy(od => od.PartNumber).ToListAsync();

                foreach (var detail in order.OrderDetails)
                {
                    //detail.VendorCatalogs = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == detail.MainPartsLinkNumber.Trim()).ToListAsync();
                    detail.VendorCatalogs = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == detail.MainPartsLinkNumber.Trim()).OrderBy(vc => vc.VendorCode).ToListAsync();

                    //Get Warehouse Stocks with Location Name
                    var stocks = (
                        from stock in _context.WarehouseStocks
                        join product in _context.Products on stock.ProductId equals product.Id
                        join location in _context.WarehouseLocations on stock.WarehouseLocationId equals location.Id
                        //where stock.ProductId == detail.ProductId && (order.Customer.State == "NV" ? true : stock.WarehouseId == 1)
                        where stock.ProductId == detail.ProductId
                        select new WarehouseStock
                        {
                            Id = stock.Id,
                            CreatedBy = stock.CreatedBy,
                            CreatedDate = stock.CreatedDate,
                            IsActive = stock.IsActive,
                            IsDeleted = stock.IsDeleted,
                            Location = location.Location,
                            ModifiedBy = stock.ModifiedBy,
                            ModifiedDate = stock.ModifiedDate,
                            ProductId = detail.ProductId,
                            Quantity = stock.Quantity > 0 ? stock.Quantity : 0,
                            WarehouseId = stock.WarehouseId,
                            WarehouseLocationId = stock.WarehouseLocationId,
                            CurrentCost = product.CurrentCost.HasValue ? product.CurrentCost.Value : 0
                        }).ToList();

                    detail.Stocks = stocks;

                    var warehouseTrackings = await _context.WarehouseTrackings.Where(e => e.OrderDetailId == detail.Id && e.IsActive && !e.IsDeleted).OrderByDescending(e => e.Id).ToListAsync();
                    detail.WarehouseTrackings = warehouseTrackings;
                }
            }

            if (recordCount > 0)
            {
                result.Data = orders;
                result.RecordCount = recordCount;
            }

            return result;
        }

        //public async Task<PaginatedListDTO<Order>> GetOrdersByDatePaginated(int pageSize, int pageIndex, DateTime fromDate, DateTime toDate)
        //{
        //    // US Settings
        //    DateTime fDate = fromDate; // new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, fromDate.Hour, fromDate.Minute, fromDate.Second);
        //    DateTime tDate = toDate.AddDays(1); // new DateTime(toDate.Year, toDate.Month, toDate.Day + 1, toDate.Hour, toDate.Minute, toDate.Second);

        //    //// PH Settings
        //    //DateTime fDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day - 1, 16, 00, 00);
        //    //DateTime tDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 16, 00, 00);

        //    var recordCount = (
        //        from order in _context.Orders
        //        join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
        //        join customer in _context.Customers on order.CustomerId equals customer.Id
        //        where order.IsActive && !order.IsDeleted && !order.IsQuote && (order.OrderDate >= fDate && order.OrderDate < tDate)
        //        select order)
        //        .Distinct()
        //        .Count();

        //    var orders = await (
        //        from order in _context.Orders
        //        join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
        //        join customer in _context.Customers on order.CustomerId equals customer.Id
        //        where order.IsActive && !order.IsDeleted && !order.IsQuote && (order.OrderDate >= fDate && order.OrderDate < tDate)
        //        select order)
        //        .Distinct()
        //        .OrderByDescending(e => e.OrderNumber)
        //        .Skip(pageSize * pageIndex)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    foreach (var order in orders)
        //    {
        //        order.Customer = await _context.Customers.Where(e => e.Id == order.CustomerId).FirstOrDefaultAsync();
        //        order.CustomerName = order.Customer != null ? order.Customer.CustomerName : "";
        //        order.AccountNumber = order.Customer != null ? order.Customer.AccountNumber : 0;

        //        var priceLevel = new PriceLevel();

        //        if (order.Customer != null)
        //        {
        //            priceLevel = await _context.PriceLevels.Where(e => e.Id == order.Customer.PriceLevelId).FirstOrDefaultAsync();
        //        }

        //        order.PriceLevelName = priceLevel != null ? priceLevel.LevelName : "";

        //        order.PaymentTerm = await _context.PaymentTerms.Where(e => e.Id == order.PaymentTermId).FirstOrDefaultAsync();
        //        order.PaymentTermName = order.PaymentTerm != null ? order.PaymentTerm.TermName : "";

        //        order.Warehouse = await _context.Warehouses.Where(e => e.Id == order.WarehouseId).FirstOrDefaultAsync();
        //        order.WarehouseName = order.Warehouse != null ? order.Warehouse.WarehouseName : "";

        //        order.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).OrderBy(od => od.PartNumber).ToListAsync();

        //        foreach (var detail in order.OrderDetails)
        //        {
        //            detail.VendorCatalogs = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == detail.MainPartsLinkNumber.Trim()).ToListAsync();

        //            //var venCat = await (
        //            //    from vc in _context.VendorCatalogs
        //            //    join v in _context.Vendors on vc.VendorCode.Trim().ToLower() equals v.VendorCode.Trim().ToLower()
        //            //    where vc.PartsLinkNumber == detail.MainPartsLinkNumber && 
        //            //    (v.IsCAVendor == (order.BillState == "CA") || v.IsNVVendor == (order.BillState == "NV"))


        //            //    select vc)
        //            //    .Distinct()
        //            //    .OrderBy(e => e.VendorCode)
        //            //    .ToListAsync();

        //            //detail.VendorCatalogs = venCat; 

        //            //Get Warehouse Stocks with Location Name
        //            var stocks = (
        //                from stock in _context.WarehouseStocks
        //                join product in _context.Products on stock.ProductId equals product.Id
        //                join location in _context.WarehouseLocations on stock.WarehouseLocationId equals location.Id
        //                //where stock.ProductId == detail.ProductId && (order.Customer.State == "NV" ? true : stock.WarehouseId == 1)
        //                where stock.ProductId == detail.ProductId
        //                select new WarehouseStock
        //                {
        //                    Id = stock.Id,
        //                    CreatedBy = stock.CreatedBy,
        //                    CreatedDate = stock.CreatedDate,
        //                    IsActive = stock.IsActive,
        //                    IsDeleted = stock.IsDeleted,
        //                    Location = location.Location,
        //                    ModifiedBy = stock.ModifiedBy,
        //                    ModifiedDate = stock.ModifiedDate,
        //                    ProductId = detail.ProductId,
        //                    Quantity = stock.Quantity > 0 ? stock.Quantity : 0,
        //                    WarehouseId = stock.WarehouseId,
        //                    WarehouseLocationId = stock.WarehouseLocationId,
        //                    CurrentCost = product.CurrentCost.HasValue ? product.CurrentCost.Value : 0
        //                }).ToList();

        //            detail.Stocks = stocks;

        //            var warehouseTrackings = await _context.WarehouseTrackings.Where(e => e.OrderDetailId == detail.Id && e.IsActive && !e.IsDeleted).OrderByDescending(e => e.Id).ToListAsync();
        //            detail.WarehouseTrackings = warehouseTrackings;
        //        }
        //    }

        //    var result = new PaginatedListDTO<Order>()
        //    {
        //        Data = orders, //.OrderByDescending(e => e.OrderNumber).ToList(),
        //        RecordCount = recordCount
        //    };

        //    return result;
        //}

        public async Task<PaginatedListDTO<Order>> GetCustomerOrdersPaginated(int customerId, int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search)
        {
            var recordCount = 0;
            var orders = new List<Order>();

            if (!string.IsNullOrEmpty(search))
            {
                recordCount = (
                    from order in _context.Orders
                    join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                    join customer in _context.Customers on order.CustomerId equals customer.Id
                    where order.CustomerId == customerId && order.IsActive && !order.IsDeleted && order.OrderStatusId != 9 && (!order.IsQuote || (order.IsQuote && order.IsCustomerOrder)) &&
                        (string.IsNullOrEmpty(search) ? true : order.OrderNumber.Value.ToString().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : order.InvoiceNumber.Value.ToString().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : order.PurchaseOrderNumber.Trim().ToLower().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : order.AccountNumber.ToString().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : order.PhoneNumber.Trim().ToLower().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : order.OrderStatusName.Trim().ToLower().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : order.ShipAddressName.Trim().ToLower().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : order.CreatedBy.Trim().ToLower().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : orderDetail.PartNumber.Trim().ToLower().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : orderDetail.MainOEMNumber.Trim().ToLower().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : orderDetail.MainPartsLinkNumber.Trim().ToLower().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : customer.CustomerName.Trim().ToLower().Contains(search))
                    select order)
                    .Distinct()
                    .Count();

                orders = await (
                    from order in _context.Orders
                    join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                    join customer in _context.Customers on order.CustomerId equals customer.Id
                    where order.CustomerId == customerId && order.IsActive && !order.IsDeleted && order.OrderStatusId != 9 && (!order.IsQuote || (order.IsQuote && order.IsCustomerOrder)) &&
                        (string.IsNullOrEmpty(search) ? true : order.OrderNumber.Value.ToString().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : order.InvoiceNumber.Value.ToString().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : order.PurchaseOrderNumber.Trim().ToLower().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : order.AccountNumber.ToString().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : order.PhoneNumber.Trim().ToLower().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : order.OrderStatusName.Trim().ToLower().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : order.ShipAddressName.Trim().ToLower().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : order.CreatedBy.Trim().ToLower().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : orderDetail.PartNumber.Trim().ToLower().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : orderDetail.MainOEMNumber.Trim().ToLower().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : orderDetail.MainPartsLinkNumber.Trim().ToLower().Contains(search) ||
                         string.IsNullOrEmpty(search) ? true : customer.CustomerName.Trim().ToLower().Contains(search))
                    select order)
                    .Distinct()
                    .OrderByDescending(e => e.Id)
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync();
            }
            else
            {
                recordCount = (
                    from order in _context.Orders
                    join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                    join customer in _context.Customers on order.CustomerId equals customer.Id
                    where order.CustomerId == customerId && order.IsActive && !order.IsDeleted && order.OrderStatusId != 9 && (!order.IsQuote || (order.IsQuote && order.IsCustomerOrder))
                    select order)
                    .Distinct()
                    .Count();

                orders = await (
                    from order in _context.Orders
                    join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                    join customer in _context.Customers on order.CustomerId equals customer.Id
                    where order.CustomerId == customerId && order.IsActive && !order.IsDeleted && order.OrderStatusId != 9 && (!order.IsQuote || (order.IsQuote && order.IsCustomerOrder))
                    select order)
                    .Distinct()
                    .OrderByDescending(e => e.Id)
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync();
            }

            foreach (var order in orders)
            {
                order.Customer = await _context.Customers.Where(e => e.Id == order.CustomerId).FirstOrDefaultAsync();
                order.CustomerName = order.Customer != null ? order.Customer.CustomerName : "";
                order.AccountNumber = order.Customer != null ? order.Customer.AccountNumber : 0;

                var priceLevel = new PriceLevel();

                if (order.Customer != null)
                {
                    priceLevel = await _context.PriceLevels.Where(e => e.Id == order.Customer.PriceLevelId).FirstOrDefaultAsync();
                }

                order.PriceLevelName = priceLevel != null ? priceLevel.LevelName : "";

                order.PaymentTerm = await _context.PaymentTerms.Where(e => e.Id == order.PaymentTermId).FirstOrDefaultAsync();
                order.PaymentTermName = order.PaymentTerm != null ? order.PaymentTerm.TermName : "";

                order.Warehouse = await _context.Warehouses.Where(e => e.Id == order.WarehouseId).FirstOrDefaultAsync();
                order.WarehouseName = order.Warehouse != null ? order.Warehouse.WarehouseName : "";

                order.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).OrderBy(od => od.PartNumber).ToListAsync();

                foreach (var detail in order.OrderDetails)
                {
                    detail.VendorCatalogs = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == detail.MainPartsLinkNumber.Trim()).ToListAsync();

                    //var venCat = await (
                    //    from vc in _context.VendorCatalogs
                    //    join v in _context.Vendors on vc.VendorCode.Trim().ToLower() equals v.VendorCode.Trim().ToLower()
                    //    where vc.PartsLinkNumber == detail.MainPartsLinkNumber && 
                    //    (v.IsCAVendor == (order.BillState == "CA") || v.IsNVVendor == (order.BillState == "NV"))


                    //    select vc)
                    //    .Distinct()
                    //    .OrderBy(e => e.VendorCode)
                    //    .ToListAsync();

                    //detail.VendorCatalogs = venCat; 

                    //Get Warehouse Stocks with Location Name
                    var stocks = (
                        from stock in _context.WarehouseStocks
                        join product in _context.Products on stock.ProductId equals product.Id
                        join location in _context.WarehouseLocations on stock.WarehouseLocationId equals location.Id
                        //where stock.ProductId == detail.ProductId && (order.Customer.State == "NV" ? true : stock.WarehouseId == 1)
                        where stock.ProductId == detail.ProductId
                        select new WarehouseStock
                        {
                            Id = stock.Id,
                            CreatedBy = stock.CreatedBy,
                            CreatedDate = stock.CreatedDate,
                            IsActive = stock.IsActive,
                            IsDeleted = stock.IsDeleted,
                            Location = location.Location,
                            ModifiedBy = stock.ModifiedBy,
                            ModifiedDate = stock.ModifiedDate,
                            ProductId = detail.ProductId,
                            Quantity = stock.Quantity > 0 ? stock.Quantity : 0,
                            WarehouseId = stock.WarehouseId,
                            WarehouseLocationId = stock.WarehouseLocationId,
                            CurrentCost = product.CurrentCost.HasValue ? product.CurrentCost.Value : 0
                        }).ToList();

                    detail.Stocks = stocks;

                    var warehouseTrackings = await _context.WarehouseTrackings.Where(e => e.OrderDetailId == detail.Id && e.IsActive && !e.IsDeleted).OrderByDescending(e => e.Id).ToListAsync();
                    detail.WarehouseTrackings = warehouseTrackings;
                }
            }

            var result = new PaginatedListDTO<Order>()
            {
                Data = orders, //.OrderByDescending(e => e.OrderNumber).ToList(),
                RecordCount = recordCount
            };

            return result;
        }

        public async Task<PaginatedListDTO<Order>> GetQuotesPaginated(int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search)
        {
            var recordCount = (
                from order in _context.Orders
                join customer in _context.Customers on order.CustomerId equals customer.Id
                where order.IsQuote && order.IsActive && !order.IsDeleted && 
                    (string.IsNullOrEmpty(search) ? true : order.QuoteNumber.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.PurchaseOrderNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.AccountNumber.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.PhoneNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.OrderStatusName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.ShipAddressName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.CreatedBy.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : customer.CustomerName.Trim().ToLower().Contains(search))
                select order)
                .Distinct()
                .Count();

            var orders = await (
                from order in _context.Orders
                join customer in _context.Customers on order.CustomerId equals customer.Id
                where order.IsQuote && order.IsActive && !order.IsDeleted &&
                    (string.IsNullOrEmpty(search) ? true : order.QuoteNumber.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.PurchaseOrderNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.AccountNumber.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.PhoneNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.OrderStatusName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.ShipAddressName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.CreatedBy.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : customer.CustomerName.Trim().ToLower().Contains(search))
                select order)
                .Distinct()
                .OrderByDescending(e => e.CreatedDate)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            foreach (var order in orders)
            {
                order.Customer = await _context.Customers.Where(e => e.Id == order.CustomerId).FirstOrDefaultAsync();
                order.CustomerName = order.Customer != null ? order.Customer.CustomerName : "";
                order.AccountNumber = order.Customer != null ? order.Customer.AccountNumber : 0;

                var priceLevel = new PriceLevel();

                if (order.Customer != null)
                {
                    priceLevel = await _context.PriceLevels.Where(e => e.Id == order.Customer.PriceLevelId).FirstOrDefaultAsync();
                }

                order.PriceLevelName = priceLevel != null ? priceLevel.LevelName : "";

                order.PaymentTerm = await _context.PaymentTerms.Where(e => e.Id == order.PaymentTermId).FirstOrDefaultAsync();
                order.PaymentTermName = order.PaymentTerm != null ? order.PaymentTerm.TermName : "";

                order.Warehouse = await _context.Warehouses.Where(e => e.Id == order.WarehouseId).FirstOrDefaultAsync();
                order.WarehouseName = order.Warehouse != null ? order.Warehouse.WarehouseName : "";

                order.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).OrderBy(od => od.PartNumber).ToListAsync();

                foreach (var detail in order.OrderDetails)
                {
                    var venCat = await (
                        from vc in _context.VendorCatalogs
                        join v in _context.Vendors on vc.VendorCode.Trim().ToLower() equals v.VendorCode.Trim().ToLower()
                        where vc.PartsLinkNumber == detail.MainPartsLinkNumber &&
                        (v.IsCAVendor == (order.BillState == "CA") || v.IsNVVendor == (order.BillState == "NV"))


                        select vc)
                        .Distinct()
                        .OrderBy(e => e.VendorCode)
                        .ToListAsync();

                    detail.VendorCatalogs = venCat;
                    //detail.VendorCatalogs = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == detail.MainPartsLinkNumber.Trim()).ToListAsync();


                    //Get Warehouse Stocks with Location Name
                    var stocks = (
                        from stock in _context.WarehouseStocks
                        join product in _context.Products on stock.ProductId equals product.Id
                        join location in _context.WarehouseLocations on stock.WarehouseLocationId equals location.Id
                        where stock.ProductId == detail.ProductId
                        select new WarehouseStock
                        {
                            Id = stock.Id,
                            CreatedBy = stock.CreatedBy,
                            CreatedDate = stock.CreatedDate,
                            IsActive = stock.IsActive,
                            IsDeleted = stock.IsDeleted,
                            Location = location.Location,
                            ModifiedBy = stock.ModifiedBy,
                            ModifiedDate = stock.ModifiedDate,
                            ProductId = detail.ProductId,
                            Quantity = stock.Quantity > 0 ? stock.Quantity : 0,
                            WarehouseId = stock.WarehouseId,
                            WarehouseLocationId = stock.WarehouseLocationId,
                            CurrentCost = product.CurrentCost.HasValue ? product.CurrentCost.Value : 0
                        }).ToList();

                    detail.Stocks = stocks;
                }
            }

            var result = new PaginatedListDTO<Order>()
            {
                Data = orders, //.OrderByDescending(e => e.OrderNumber).ToList(),
                RecordCount = recordCount
            };

            return result;
        }

        public async Task<PaginatedListDTO<Order>> GetWebOrdersPaginated(int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search)
        {
            var recordCount = (
                from order in _context.Orders
                join customer in _context.Customers on order.CustomerId equals customer.Id
                where order.IsQuote && order.IsCustomerOrder && order.IsActive && !order.IsDeleted &&
                    (string.IsNullOrEmpty(search) ? true : order.QuoteNumber.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.PurchaseOrderNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.AccountNumber.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.PhoneNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.OrderStatusName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.ShipAddressName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.CreatedBy.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : customer.CustomerName.Trim().ToLower().Contains(search))
                select order)
                .Distinct()
                .Count();

            var orders = await (
                from order in _context.Orders
                join customer in _context.Customers on order.CustomerId equals customer.Id
                where order.IsQuote && order.IsCustomerOrder && order.IsActive && !order.IsDeleted &&
                    (string.IsNullOrEmpty(search) ? true : order.QuoteNumber.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.PurchaseOrderNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.AccountNumber.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.PhoneNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.OrderStatusName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.ShipAddressName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.CreatedBy.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : customer.CustomerName.Trim().ToLower().Contains(search))
                select order)
                .Distinct()
                .OrderByDescending(e => e.CreatedDate)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            foreach (var order in orders)
            {
                order.Customer = await _context.Customers.Where(e => e.Id == order.CustomerId).FirstOrDefaultAsync();
                order.CustomerName = order.Customer != null ? order.Customer.CustomerName : "";
                order.AccountNumber = order.Customer != null ? order.Customer.AccountNumber : 0;

                var priceLevel = new PriceLevel();

                if (order.Customer != null)
                {
                    priceLevel = await _context.PriceLevels.Where(e => e.Id == order.Customer.PriceLevelId).FirstOrDefaultAsync();
                }

                order.PriceLevelName = priceLevel != null ? priceLevel.LevelName : "";

                order.PaymentTerm = await _context.PaymentTerms.Where(e => e.Id == order.PaymentTermId).FirstOrDefaultAsync();
                order.PaymentTermName = order.PaymentTerm != null ? order.PaymentTerm.TermName : "";

                order.Warehouse = await _context.Warehouses.Where(e => e.Id == order.WarehouseId).FirstOrDefaultAsync();
                order.WarehouseName = order.Warehouse != null ? order.Warehouse.WarehouseName : "";

                order.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).OrderBy(od => od.PartNumber).ToListAsync();

                foreach (var detail in order.OrderDetails)
                {
                    var venCat = await (
                        from vc in _context.VendorCatalogs
                        join v in _context.Vendors on vc.VendorCode.Trim().ToLower() equals v.VendorCode.Trim().ToLower()
                        where vc.PartsLinkNumber == detail.MainPartsLinkNumber &&
                        (v.IsCAVendor == (order.BillState == "CA") || v.IsNVVendor == (order.BillState == "NV"))


                        select vc)
                        .Distinct()
                        .OrderBy(e => e.VendorCode)
                        .ToListAsync();

                    detail.VendorCatalogs = venCat;
                    //detail.VendorCatalogs = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == detail.MainPartsLinkNumber.Trim()).ToListAsync();


                    //Get Warehouse Stocks with Location Name
                    var stocks = (
                        from stock in _context.WarehouseStocks
                        join product in _context.Products on stock.ProductId equals product.Id
                        join location in _context.WarehouseLocations on stock.WarehouseLocationId equals location.Id
                        where stock.ProductId == detail.ProductId
                        select new WarehouseStock
                        {
                            Id = stock.Id,
                            CreatedBy = stock.CreatedBy,
                            CreatedDate = stock.CreatedDate,
                            IsActive = stock.IsActive,
                            IsDeleted = stock.IsDeleted,
                            Location = location.Location,
                            ModifiedBy = stock.ModifiedBy,
                            ModifiedDate = stock.ModifiedDate,
                            ProductId = detail.ProductId,
                            Quantity = stock.Quantity > 0 ? stock.Quantity : 0,
                            WarehouseId = stock.WarehouseId,
                            WarehouseLocationId = stock.WarehouseLocationId,
                            CurrentCost = product.CurrentCost.HasValue ? product.CurrentCost.Value : 0
                        }).ToList();

                    detail.Stocks = stocks;
                }
            }

            var result = new PaginatedListDTO<Order>()
            {
                Data = orders, //.OrderByDescending(e => e.OrderNumber).ToList(),
                RecordCount = recordCount
            };

            return result;
        }

        public async Task<PaginatedListDTO<Order>> GetRGAOrdersPaginated(int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search)
        {
            var recordCount = (
                from order in _context.Orders
                join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                join customer in _context.Customers on order.CustomerId equals customer.Id
                where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId == 9 &&
                    (string.IsNullOrEmpty(search) ? true : order.OrderNumber.Value.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.InvoiceNumber.Value.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.PurchaseOrderNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.AccountNumber.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.PhoneNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.OrderStatusName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.ShipAddressName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.CreatedBy.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : orderDetail.PartNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : orderDetail.MainOEMNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : orderDetail.MainPartsLinkNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : customer.CustomerName.Trim().ToLower().Contains(search))
                select order)
                .Distinct()
                .Count();

            var orders = await (
                from order in _context.Orders
                join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                join customer in _context.Customers on order.CustomerId equals customer.Id
                where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId == 9 &&
                    (string.IsNullOrEmpty(search) ? true : order.OrderNumber.Value.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.InvoiceNumber.Value.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.PurchaseOrderNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.AccountNumber.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.PhoneNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.OrderStatusName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.ShipAddressName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.CreatedBy.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : orderDetail.PartNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : orderDetail.MainOEMNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : orderDetail.MainPartsLinkNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : customer.CustomerName.Trim().ToLower().Contains(search))
                select order)
                .Distinct()
                .OrderByDescending(e => e.OrderNumber)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            foreach (var order in orders)
            {
                order.Customer = await _context.Customers.Where(e => e.Id == order.CustomerId).FirstOrDefaultAsync();
                order.CustomerName = order.Customer != null ? order.Customer.CustomerName : "";
                order.AccountNumber = order.Customer != null ? order.Customer.AccountNumber : 0;

                var priceLevel = new PriceLevel();

                if (order.Customer != null)
                {
                    priceLevel = await _context.PriceLevels.Where(e => e.Id == order.Customer.PriceLevelId).FirstOrDefaultAsync();
                }

                order.PriceLevelName = priceLevel != null ? priceLevel.LevelName : "";

                order.PaymentTerm = await _context.PaymentTerms.Where(e => e.Id == order.PaymentTermId).FirstOrDefaultAsync();
                order.PaymentTermName = order.PaymentTerm != null ? order.PaymentTerm.TermName : "";

                order.Warehouse = await _context.Warehouses.Where(e => e.Id == order.WarehouseId).FirstOrDefaultAsync();
                order.WarehouseName = order.Warehouse != null ? order.Warehouse.WarehouseName : "";

                order.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).OrderBy(od => od.PartNumber).ToListAsync();

                foreach (var detail in order.OrderDetails)
                {
                    detail.VendorCatalogs = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == detail.MainPartsLinkNumber.Trim()).ToListAsync();

                    //var venCat = await (
                    //    from vc in _context.VendorCatalogs
                    //    join v in _context.Vendors on vc.VendorCode.Trim().ToLower() equals v.VendorCode.Trim().ToLower()
                    //    where vc.PartsLinkNumber == detail.MainPartsLinkNumber && 
                    //    (v.IsCAVendor == (order.BillState == "CA") || v.IsNVVendor == (order.BillState == "NV"))


                    //    select vc)
                    //    .Distinct()
                    //    .OrderBy(e => e.VendorCode)
                    //    .ToListAsync();

                    //detail.VendorCatalogs = venCat; 

                    //Get Warehouse Stocks with Location Name
                    var stocks = (
                        from stock in _context.WarehouseStocks
                        join product in _context.Products on stock.ProductId equals product.Id
                        join location in _context.WarehouseLocations on stock.WarehouseLocationId equals location.Id
                        //where stock.ProductId == detail.ProductId && (order.Customer.State == "NV" ? true : stock.WarehouseId == 1)
                        where stock.ProductId == detail.ProductId
                        select new WarehouseStock
                        {
                            Id = stock.Id,
                            CreatedBy = stock.CreatedBy,
                            CreatedDate = stock.CreatedDate,
                            IsActive = stock.IsActive,
                            IsDeleted = stock.IsDeleted,
                            Location = location.Location,
                            ModifiedBy = stock.ModifiedBy,
                            ModifiedDate = stock.ModifiedDate,
                            ProductId = detail.ProductId,
                            Quantity = stock.Quantity > 0 ? stock.Quantity : 0,
                            WarehouseId = stock.WarehouseId,
                            WarehouseLocationId = stock.WarehouseLocationId,
                            CurrentCost = product.CurrentCost.HasValue ? product.CurrentCost.Value : 0
                        }).ToList();

                    detail.Stocks = stocks;

                    var warehouseTrackings = await _context.WarehouseTrackings.Where(e => e.OrderDetailId == detail.Id && e.IsActive && !e.IsDeleted).OrderByDescending(e => e.Id).ToListAsync();
                    detail.WarehouseTrackings = warehouseTrackings;
                }
            }

            var result = new PaginatedListDTO<Order>()
            {
                Data = orders, //.OrderByDescending(e => e.OrderNumber).ToList(),
                RecordCount = recordCount
            };

            return result;
        }

        public async Task<PaginatedListDTO<Order>> GetReportOrdersListPaginated(DateTime deliveryDate, int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search, string? state, int? deliveryRoute)
        {
            var recordCount = (
                from order in _context.Orders
                join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                join customer in _context.Customers on order.CustomerId equals customer.Id
                where order.IsActive && !order.IsDeleted && !order.IsQuote && !order.IsPrinted && order.OrderStatusId != 3 && order.DeliveryDate.Date == deliveryDate.ToUniversalTime().Date &&
                    (string.IsNullOrEmpty(state) ? true : order.BillState.Trim().ToLower() == state.Trim().ToLower()) && 
                    (deliveryRoute == null || deliveryRoute == 0 ? true : order.DeliveryRoute == deliveryRoute) &&
                    (string.IsNullOrEmpty(search) ? true : order.OrderNumber.Value.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.InvoiceNumber.Value.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.PurchaseOrderNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.AccountNumber.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.PhoneNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.OrderStatusName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.ShipAddressName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.CreatedBy.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : orderDetail.PartNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : orderDetail.MainOEMNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : orderDetail.MainPartsLinkNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : customer.CustomerName.Trim().ToLower().Contains(search))
                select order)
                .Distinct()
                .Count();

            var orders = await (
                from order in _context.Orders
                join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                join customer in _context.Customers on order.CustomerId equals customer.Id
                where order.IsActive && !order.IsDeleted && !order.IsQuote && !order.IsPrinted && order.OrderStatusId != 3 && order.DeliveryDate.Date == deliveryDate.ToUniversalTime().Date &&
                    (string.IsNullOrEmpty(state) ? true : order.BillState.Trim().ToLower() == state.Trim().ToLower()) &&
                    (deliveryRoute == null || deliveryRoute == 0 ? true : order.DeliveryRoute == deliveryRoute) &&
                    (string.IsNullOrEmpty(search) ? true : order.OrderNumber.Value.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.InvoiceNumber.Value.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.PurchaseOrderNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.AccountNumber.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.PhoneNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.OrderStatusName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.ShipAddressName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : order.CreatedBy.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : orderDetail.PartNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : orderDetail.MainOEMNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : orderDetail.MainPartsLinkNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : customer.CustomerName.Trim().ToLower().Contains(search))
                select order)
                .Distinct()
                .OrderBy(e => e.OrderNumber)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            foreach (var order in orders)
            {
                order.Customer = await _context.Customers.Where(e => e.Id == order.CustomerId).FirstOrDefaultAsync();
                order.CustomerName = order.Customer != null ? order.Customer.CustomerName : "";
                order.AccountNumber = order.Customer != null ? order.Customer.AccountNumber : 0;

                var priceLevel = new PriceLevel();

                if (order.Customer != null)
                {
                    priceLevel = await _context.PriceLevels.Where(e => e.Id == order.Customer.PriceLevelId).FirstOrDefaultAsync();
                }

                order.PriceLevelName = priceLevel != null ? priceLevel.LevelName : "";

                order.PaymentTerm = await _context.PaymentTerms.Where(e => e.Id == order.PaymentTermId).FirstOrDefaultAsync();
                order.PaymentTermName = order.PaymentTerm != null ? order.PaymentTerm.TermName : "";

                order.Warehouse = await _context.Warehouses.Where(e => e.Id == order.WarehouseId).FirstOrDefaultAsync();
                order.WarehouseName = order.Warehouse != null ? order.Warehouse.WarehouseName : "";

                order.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).OrderBy(od => od.PartNumber).ToListAsync();

                foreach (var detail in order.OrderDetails)
                {
                    detail.VendorCatalogs = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == detail.MainPartsLinkNumber.Trim()).ToListAsync();
                    //detail.Stocks = await _context.WarehouseStocks.Where(e => e.ProductId == detail.ProductId).ToListAsync();

                    //Get Warehouse Stocks with Location Name
                    var stocks = (
                        from stock in _context.WarehouseStocks
                        join product in _context.Products on stock.ProductId equals product.Id
                        join location in _context.WarehouseLocations on stock.WarehouseLocationId equals location.Id
                        where stock.ProductId == detail.ProductId
                        select new WarehouseStock
                        {
                            Id = stock.Id,
                            CreatedBy = stock.CreatedBy,
                            CreatedDate = stock.CreatedDate,
                            IsActive = stock.IsActive,
                            IsDeleted = stock.IsDeleted,
                            Location = location.Location,
                            ModifiedBy = stock.ModifiedBy,
                            ModifiedDate = stock.ModifiedDate,
                            ProductId = detail.ProductId,
                            Quantity = stock.Quantity > 0 ? stock.Quantity : 0,
                            WarehouseId = stock.WarehouseId,
                            WarehouseLocationId = stock.WarehouseLocationId,
                            CurrentCost = product.CurrentCost.HasValue ? product.CurrentCost.Value : 0
                        }).ToList();

                    detail.Stocks = stocks;
                }
            }

            var result = new PaginatedListDTO<Order>()
            {
                Data = orders,
                RecordCount = recordCount
            };

            return result;
        }


        public async Task<Order?> GetOrder(int orderId)
        {
            var result = await _context.Orders.FindAsync(orderId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<DailySalesSummaryDTO> GetDailySalesSummary(DateTime currentDate)
        {
            var result = new DailySalesSummaryDTO();

            try
            {
                // US Settings
                //DateTime frDate = DateTime.Parse("2024-03-11 08:00:00");
                //DateTime toDate = frDate.AddDays(1);

                DateTime frDate = currentDate; //new DateTime(dt.Year, dt.Month, dt.Day, 8, 00, 00);
                DateTime toDate = currentDate.AddDays(1); //new DateTime(dt.Year, dt.Month, dt.Day + 1, 8, 00, 00);

                #region California
                var orders = await _context.Orders.Where(e => (e.OrderDate >= frDate && e.OrderDate < toDate) && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                    !(e.OrderStatusId == 3 || e.OrderStatusId == 9) && e.BillState.Trim() == "CA").ToListAsync();
                foreach (var order in orders)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(e => e.UserName.Trim().ToLower() == order.CreatedBy.Trim().ToLower());
                    if (user != null)
                    {
                        if (user.IsCustomerUser)
                        {
                            user = await _context.Users.FirstOrDefaultAsync(e => e.UserName.Trim().ToLower() == order.ModifiedBy.Trim().ToLower());
                        }
                    }

                    if (user != null && !user.IsCustomerUser)
                    {
                        var sp = result.CASummary.Find(e => e.SalesPerson.Trim().ToLower() == user.UserName.Trim().ToLower());
                        var orderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).ToListAsync();
                        if (sp != null)
                        {
                            sp.SalesAmount += orderDetails.Where(od => od.TotalAmount > 0).Sum(e => e.TotalAmount);
                            sp.CreditAmount += orderDetails.Where(od => od.TotalAmount < 0).Sum(e => e.TotalAmount);
                            sp.NetSalesAmount += order.TotalAmount != null ? order.TotalAmount.Value : 0;
                            sp.UnitCost += orderDetails.Where(od => od.UnitCost != null).Sum(e => e.UnitCost.Value);
                        }
                        else
                        {
                            result.CASummary.Add(new TotalSalesDTO()
                            {
                                SalesAmount = orderDetails.Where(od => od.TotalAmount > 0).Sum(e => e.TotalAmount),
                                CreditAmount = orderDetails.Where(od => od.TotalAmount < 0).Sum(e => e.TotalAmount),
                                NetSalesAmount = order.TotalAmount != null ? order.TotalAmount.Value : 0,
                                UnitCost = orderDetails.Where(od => od.UnitCost != null).Sum(e => e.UnitCost.Value),
                                SalesPerson = user.UserName.Trim()
                            });
                        }
                    }
                }

                foreach (var sales in result.CASummary)
                {
                    try
                    {
                        sales.ProfitMargin = Math.Abs((sales.NetSalesAmount - sales.UnitCost) / sales.NetSalesAmount * 100);
                        sales.ReturnPercentage = (sales.SalesAmount != 0 || sales.CreditAmount != 0) ? Math.Abs((Math.Abs(sales.CreditAmount) / sales.SalesAmount)) * 100 : 0;
                    }
                    catch { }
                }

                try
                {
                    var totalNet = result.CASummary.Sum(e => e.NetSalesAmount);
                    var totalCost = result.CASummary.Sum(e => e.UnitCost);
                    result.CATotalProfitMargin = (totalNet != 0 && totalCost != 0) ? Math.Abs((totalNet - totalCost) / totalNet * 100) : 0;

                    var totalSales = result.CASummary.Sum(e => e.SalesAmount);
                    var totalCredit = result.CASummary.Sum(e => e.CreditAmount);
                    result.CATotalReturnPercentage = (totalSales != 0 && totalCredit != 0) ? Math.Abs((Math.Abs(totalCredit) / totalSales)) * 100 : 0;
                }
                catch { }
                
                #endregion

                #region Nevada
                orders = await _context.Orders.Where(e => (e.OrderDate >= frDate && e.OrderDate < toDate) && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                    !(e.OrderStatusId == 3 || e.OrderStatusId == 9) && e.BillState.Trim() == "NV").ToListAsync();
                foreach (var order in orders)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(e => e.UserName.Trim().ToLower() == order.CreatedBy.Trim().ToLower());
                    if (user != null)
                    {
                        if (user.IsCustomerUser)
                        {
                            user = await _context.Users.FirstOrDefaultAsync(e => e.UserName.Trim().ToLower() == order.ModifiedBy.Trim().ToLower());
                        }
                    }

                    if (user != null && !user.IsCustomerUser)
                    {
                        var sp = result.NVSummary.Find(e => e.SalesPerson.Trim().ToLower() == user.UserName.Trim().ToLower());
                        var orderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).ToListAsync();
                        if (sp != null)
                        {
                            sp.SalesAmount += orderDetails.Where(od => od.TotalAmount > 0).Sum(e => e.TotalAmount);
                            sp.CreditAmount += orderDetails.Where(od => od.TotalAmount < 0).Sum(e => e.TotalAmount);
                            sp.NetSalesAmount += order.TotalAmount != null ? order.TotalAmount.Value : 0;
                            sp.UnitCost += orderDetails.Where(od => od.UnitCost != null).Sum(e => e.UnitCost.Value);
                        }
                        else
                        {
                            result.NVSummary.Add(new TotalSalesDTO()
                            {
                                SalesAmount = orderDetails.Where(od => od.TotalAmount > 0).Sum(e => e.TotalAmount),
                                CreditAmount = orderDetails.Where(od => od.TotalAmount < 0).Sum(e => e.TotalAmount),
                                NetSalesAmount = order.TotalAmount != null ? order.TotalAmount.Value : 0,
                                UnitCost = orderDetails.Where(od => od.UnitCost != null).Sum(e => e.UnitCost.Value),
                                SalesPerson = user.UserName.Trim()
                            });
                        }
                    }
                }

                foreach (var sales in result.NVSummary)
                {
                    try
                    {
                        sales.ProfitMargin = Math.Abs((sales.NetSalesAmount - sales.UnitCost) / sales.NetSalesAmount * 100);
                        sales.ReturnPercentage = (sales.SalesAmount != 0 || sales.CreditAmount != 0) ? Math.Abs((Math.Abs(sales.CreditAmount) / sales.SalesAmount)) * 100 : 0;
                    }
                    catch { }
                }

                try
                {
                    var totalNet = result.NVSummary.Sum(e => e.NetSalesAmount);
                    var totalCost = result.NVSummary.Sum(e => e.UnitCost);
                    result.NVTotalProfitMargin = (totalNet != 0 && totalCost != 0) ? Math.Abs((totalNet - totalCost) / totalNet * 100) : 0;
                    
                    var totalSales = result.NVSummary.Sum(e => e.SalesAmount);
                    var totalCredit = result.NVSummary.Sum(e => e.CreditAmount);
                    result.NVTotalReturnPercentage = (totalSales != 0 && totalCredit != 0) ? Math.Abs((Math.Abs(totalCredit) / totalSales)) * 100 : 0;
                }
                catch { }
                #endregion

            }
            catch (Exception ex)
            {
                return result;
            }

            return result;
        }

        public async Task<DailySalesSummaryDTO> GetDailySalesSummaryByDate(DateTime fromDate, DateTime toDate)
        {
            var result = new DailySalesSummaryDTO();

            try
            {
                // US Settings
                //DateTime fDate = DateTime.Parse("2024-03-11 08:00:00");
                //DateTime tDate = frDate.AddDays(1);

                DateTime fDate = fromDate; // new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, fromDate.Hour, fromDate.Minute, fromDate.Second);
                DateTime tDate = toDate.AddDays(1); // new DateTime(toDate.Year, toDate.Month, toDate.Day + 1, toDate.Hour, toDate.Minute, toDate.Second);

                #region California
                var orders = await _context.Orders.Where(e => (e.OrderDate >= fDate && e.OrderDate < tDate) && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                    !(e.OrderStatusId == 3 || e.OrderStatusId == 9) && e.BillState.Trim() == "CA").ToListAsync();

                foreach (var order in orders)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(e => e.UserName.Trim().ToLower() == order.CreatedBy.Trim().ToLower());
                    if (user != null)
                    {
                        if (user.IsCustomerUser)
                        {
                            user = await _context.Users.FirstOrDefaultAsync(e => e.UserName.Trim().ToLower() == order.ModifiedBy.Trim().ToLower());
                        }
                    }

                    if (user != null && !user.IsCustomerUser)
                    {
                        var sp = result.CASummary.Find(e => e.SalesPerson.Trim().ToLower() == user.UserName.Trim().ToLower());
                        var orderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).ToListAsync();
                        if (sp != null)
                        {
                            sp.SalesAmount += orderDetails.Where(od => od.TotalAmount > 0).Sum(e => e.TotalAmount);
                            sp.CreditAmount += orderDetails.Where(od => od.TotalAmount < 0).Sum(e => e.TotalAmount);
                            sp.NetSalesAmount += order.TotalAmount != null ? order.TotalAmount.Value : 0;
                            sp.UnitCost += orderDetails.Where(od => od.UnitCost != null).Sum(e => e.UnitCost.Value);
                        }
                        else
                        {
                            result.CASummary.Add(new TotalSalesDTO()
                            {
                                SalesAmount = orderDetails.Where(od => od.TotalAmount > 0).Sum(e => e.TotalAmount),
                                CreditAmount = orderDetails.Where(od => od.TotalAmount < 0).Sum(e => e.TotalAmount),
                                NetSalesAmount = order.TotalAmount != null ? order.TotalAmount.Value : 0,
                                UnitCost = orderDetails.Where(od => od.UnitCost != null).Sum(e => e.UnitCost.Value),
                                SalesPerson = user.UserName.Trim()
                            });
                        }
                    }
                }

                foreach (var sales in result.CASummary)
                {
                    try
                    {
                        sales.ProfitMargin = Math.Abs((sales.NetSalesAmount - sales.UnitCost) / sales.NetSalesAmount * 100);
                        sales.ReturnPercentage = (sales.SalesAmount != 0 || sales.CreditAmount != 0) ? Math.Abs((Math.Abs(sales.CreditAmount) / sales.SalesAmount)) * 100 : 0;
                    }
                    catch { }
                }

                try
                {
                    var totalNet = result.CASummary.Sum(e => e.NetSalesAmount);
                    var totalCost = result.CASummary.Sum(e => e.UnitCost);
                    result.CATotalProfitMargin = (totalNet != 0 && totalCost != 0) ? Math.Abs((totalNet - totalCost) / totalNet * 100) : 0;

                    var totalSales = result.CASummary.Sum(e => e.SalesAmount);
                    var totalCredit = result.CASummary.Sum(e => e.CreditAmount);
                    result.CATotalReturnPercentage = (totalSales != 0 && totalCredit != 0) ? Math.Abs((Math.Abs(totalCredit) / totalSales)) * 100 : 0;
                }
                catch { }
                #endregion

                #region Nevada
                orders = await _context.Orders.Where(e => (e.OrderDate >= fDate && e.OrderDate < tDate) && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                    !(e.OrderStatusId == 3 || e.OrderStatusId == 9) && e.BillState.Trim() == "NV").ToListAsync();
                foreach (var order in orders)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(e => e.UserName.Trim().ToLower() == order.CreatedBy.Trim().ToLower());
                    if (user != null)
                    {
                        if (user.IsCustomerUser)
                        {
                            user = await _context.Users.FirstOrDefaultAsync(e => e.UserName.Trim().ToLower() == order.ModifiedBy.Trim().ToLower());
                        }
                    }

                    if (user != null && !user.IsCustomerUser)
                    {
                        var sp = result.NVSummary.Find(e => e.SalesPerson.Trim().ToLower() == user.UserName.Trim().ToLower());
                        var orderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).ToListAsync();
                        if (sp != null)
                        {
                            sp.SalesAmount += orderDetails.Where(od => od.TotalAmount > 0).Sum(e => e.TotalAmount);
                            sp.CreditAmount += orderDetails.Where(od => od.TotalAmount < 0).Sum(e => e.TotalAmount);
                            sp.NetSalesAmount += order.TotalAmount != null ? order.TotalAmount.Value : 0;
                            sp.UnitCost += orderDetails.Where(od => od.UnitCost != null).Sum(e => e.UnitCost.Value);
                        }
                        else
                        {
                            result.NVSummary.Add(new TotalSalesDTO()
                            {
                                SalesAmount = orderDetails.Where(od => od.TotalAmount > 0).Sum(e => e.TotalAmount),
                                CreditAmount = orderDetails.Where(od => od.TotalAmount < 0).Sum(e => e.TotalAmount),
                                NetSalesAmount = order.TotalAmount != null ? order.TotalAmount.Value : 0,
                                UnitCost = orderDetails.Where(od => od.UnitCost != null).Sum(e => e.UnitCost.Value),
                                SalesPerson = user.UserName.Trim()
                            });
                        }
                    }
                }

                foreach (var sales in result.NVSummary)
                {
                    try
                    {
                        sales.ProfitMargin = Math.Abs((sales.NetSalesAmount - sales.UnitCost) / sales.NetSalesAmount * 100);
                        sales.ReturnPercentage = (sales.SalesAmount != 0 || sales.CreditAmount != 0) ? Math.Abs((Math.Abs(sales.CreditAmount) / sales.SalesAmount)) * 100 : 0;
                    }
                    catch { }
                }

                try
                {
                    var totalNet = result.NVSummary.Sum(e => e.NetSalesAmount);
                    var totalCost = result.NVSummary.Sum(e => e.UnitCost);
                    result.NVTotalProfitMargin = (totalNet != 0 && totalCost != 0) ? Math.Abs((totalNet - totalCost) / totalNet * 100) : 0;

                    var totalSales = result.NVSummary.Sum(e => e.SalesAmount);
                    var totalCredit = result.NVSummary.Sum(e => e.CreditAmount);
                    result.NVTotalReturnPercentage = (totalSales != 0 && totalCredit != 0) ? Math.Abs((Math.Abs(totalCredit) / totalSales)) * 100 : 0;
                }
                catch { }
                #endregion

            }
            catch (Exception ex)
            {
                return result;
            }

            return result;
        }

        //public async Task<List<TotalSalesDTO>> GetDailyTotalSalesSummaryCA()
        //{
        //    var result = new List<TotalSalesDTO>();
        //    DateTime dt = DateTime.Now.ToUniversalTime();
        //    var orders = await _context.Orders.Where(e => e.OrderDate.Date == dt.Date && !e.IsQuote && e.IsActive && !e.IsDeleted &&
        //                                             e.OrderStatusId != 3 && e.BillState.Trim().ToLower() == "ca").ToListAsync();

        //    foreach (var order in orders)
        //    {
        //        var sp = result.Find(e => e.SalesPerson.Trim().ToLower() == order.CreatedBy.Trim().ToLower());
        //        var orderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).ToListAsync();
        //        if (sp != null)
        //        {
        //            sp.SalesAmount += orderDetails.Where(od => od.TotalAmount > 0).Sum(e => e.TotalAmount);
        //            sp.CreditAmount += orderDetails.Where(od => od.TotalAmount < 0).Sum(e => e.TotalAmount);
        //            sp.NetSalesAmount += order.TotalAmount != null ? order.TotalAmount.Value : 0;
        //            sp.UnitCost += orderDetails.Where(od => od.UnitCost != null).Sum(e => e.UnitCost.Value);
        //        }
        //        else
        //        {
        //            result.Add(new TotalSalesDTO()
        //            {
        //                SalesAmount = orderDetails.Where(od => od.TotalAmount > 0).Sum(e => e.TotalAmount),
        //                CreditAmount = orderDetails.Where(od => od.TotalAmount < 0).Sum(e => e.TotalAmount),
        //                NetSalesAmount = order.TotalAmount != null ? order.TotalAmount.Value : 0,
        //                UnitCost = orderDetails.Where(od => od.UnitCost != null).Sum(e => e.UnitCost.Value),
        //                SalesPerson = order.CreatedBy
        //            });
        //        }
        //    }

        //    foreach(var sales in result)
        //    {
        //        sales.ProfitMargin = (sales.NetSalesAmount - sales.UnitCost) / sales.NetSalesAmount * 100;
        //    }

        //    return result;
        //}

        //public async Task<List<TotalSalesDTO>> GetDailyTotalSalesSummaryNV()
        //{
        //    var result = new List<TotalSalesDTO>();
        //    DateTime dt = DateTime.Now.ToUniversalTime();
        //    var orders = await _context.Orders.Where(e => e.OrderDate.Date == dt.Date && !e.IsQuote && e.IsActive && !e.IsDeleted &&
        //                                             e.OrderStatusId != 3 && e.BillState.Trim().ToLower() == "nv").ToListAsync();

        //    foreach (var order in orders)
        //    {
        //        var sp = result.Find(e => e.SalesPerson.Trim().ToLower() == order.CreatedBy.Trim().ToLower());
        //        var orderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).ToListAsync();
        //        if (sp != null)
        //        {
        //            sp.SalesAmount += orderDetails.Where(od => od.TotalAmount > 0).Sum(e => e.TotalAmount);
        //            sp.CreditAmount += orderDetails.Where(od => od.TotalAmount < 0).Sum(e => e.TotalAmount);
        //            sp.NetSalesAmount += order.TotalAmount != null ? order.TotalAmount.Value : 0;
        //            sp.UnitCost += orderDetails.Where(od => od.UnitCost != null).Sum(e => e.UnitCost.Value);
        //        }
        //        else
        //        {
        //            result.Add(new TotalSalesDTO()
        //            {
        //                SalesAmount = orderDetails.Where(od => od.TotalAmount > 0).Sum(e => e.TotalAmount),
        //                CreditAmount = orderDetails.Where(od => od.TotalAmount < 0).Sum(e => e.TotalAmount),
        //                NetSalesAmount = order.TotalAmount != null ? order.TotalAmount.Value : 0,
        //                UnitCost = orderDetails.Where(od => od.UnitCost != null).Sum(e => e.UnitCost.Value),
        //                SalesPerson = order.CreatedBy
        //            });
        //        }
        //    }

        //    foreach (var sales in result)
        //    {
        //        sales.ProfitMargin = (sales.NetSalesAmount - sales.UnitCost) / sales.NetSalesAmount * 100;
        //    }

        //    return result;
        //}
        public async Task<List<AgingBalanceReportDTO>> GetAgingBalanceReport(DateTime reportDate)
        {
            List<AgingBalanceReportDTO> result = new List<AgingBalanceReportDTO>();
            var cutOffDate = reportDate.AddDays(1);

            try
            {
                var paymentTerms = await _context.PaymentTerms.ToListAsync();

                // Get Current30Days
                var current30Days = await _context.Orders.Where(e => e.OrderDate.Date <= cutOffDate.Date && e.OrderDate.Date >= cutOffDate.AddDays(-30).Date && 
                e.IsActive && !e.IsDeleted && !e.IsQuote && !(e.OrderStatusId == 3 || e.OrderStatusId == 6 || e.OrderStatusId == 9) && e.Balance != 0).ToListAsync();

                foreach (var order in current30Days)
                {
                    var paymentTermName = string.Empty;

                    var paymentTerm = paymentTerms.FirstOrDefault(e => e.Id == order.PaymentTermId);

                    if (paymentTerm != null)
                    {
                        paymentTermName = paymentTerm.TermName;
                    }

                    //Check if customer is already in result
                    var record = result.FirstOrDefault(e => e.CustomerId == order.CustomerId);
                    if (record == null)
                    {
                        // Get last Payment of Customer
                        var payment = await _context.Payments.Where(e => e.CustomerId == order.CustomerId).OrderByDescending(o => o.Id).FirstOrDefaultAsync();
                        
                        // Get last Customer Note of Customer
                        var customerNote = await _context.CustomerNotes.Where(e => e.CustomerId == order.CustomerId).OrderByDescending(o => o.Id).FirstOrDefaultAsync();

                        result.Add(new AgingBalanceReportDTO()
                        {
                            AccountNumber = order.AccountNumber,
                            CustomerId = order.CustomerId,
                            CustomerName = order.CustomerName,
                            PhoneNumber = order.PhoneNumber,
                            Email = order.Email != null ? order.Email : string.Empty,
                            State = order.BillState,
                            Current30Days = order.Balance,
                            Over30Days = 0,
                            Over60Days = 0,
                            Over90Days = 0,
                            TotalOwed = order.Balance,
                            LastPaymentDate = payment != null ? payment.PaymentDate : null,
                            PaymentTermId = order.PaymentTermId != null ? order.PaymentTermId.Value : 0,
                            PaymentTermName = paymentTermName,
                            LastUpdateDate = customerNote != null ? customerNote.CreatedDate : null
                        });
                    }
                    else
                    {
                        record.Current30Days += order.Balance;
                        record.TotalOwed += order.Balance;
                    }
                }

                // Get Over30Days
                var over30Days = await _context.Orders.Where(e => e.OrderDate.Date <= cutOffDate.AddDays(-31).Date && e.OrderDate.Date >= cutOffDate.AddDays(-60).Date &&
                e.IsActive && !e.IsDeleted && !e.IsQuote && !(e.OrderStatusId == 3 || e.OrderStatusId == 6 || e.OrderStatusId == 9) && e.Balance != 0).ToListAsync();

                foreach (var order in over30Days)
                {
                    var paymentTermName = string.Empty;

                    var paymentTerm = paymentTerms.FirstOrDefault(e => e.Id == order.PaymentTermId);

                    if (paymentTerm != null)
                    {
                        paymentTermName = paymentTerm.TermName;
                    }

                    //Check if customer is already in result
                    var record = result.FirstOrDefault(e => e.CustomerId == order.CustomerId);
                    if (record == null)
                    {
                        // Get last Payment of Customer
                        var payment = await _context.Payments.Where(e => e.CustomerId == order.CustomerId).OrderByDescending(o => o.Id).FirstOrDefaultAsync();

                        // Get last Customer Note of Customer
                        var customerNote = await _context.CustomerNotes.Where(e => e.CustomerId == order.CustomerId).OrderByDescending(o => o.Id).FirstOrDefaultAsync();

                        result.Add(new AgingBalanceReportDTO()
                        {
                            AccountNumber = order.AccountNumber,
                            CustomerId = order.CustomerId,
                            CustomerName = order.CustomerName,
                            PhoneNumber = order.PhoneNumber,
                            Email = order.Email != null ? order.Email : string.Empty,
                            State = order.BillState,
                            Current30Days = 0,
                            Over30Days = order.Balance,
                            Over60Days = 0,
                            Over90Days = 0,
                            TotalOwed = order.Balance,
                            LastPaymentDate = payment != null ? payment.PaymentDate : null,
                            PaymentTermName = paymentTermName,
                            LastUpdateDate = customerNote != null ? customerNote.CreatedDate : null
                        });
                    }
                    else
                    {
                        record.Over30Days += order.Balance;
                        record.TotalOwed += order.Balance;
                    }
                }

                // Get Over60Days
                var over60Days = await _context.Orders.Where(e => e.OrderDate.Date <= cutOffDate.AddDays(-61).Date && e.OrderDate.Date >= cutOffDate.AddDays(-90).Date &&
                e.IsActive && !e.IsDeleted && !e.IsQuote && !(e.OrderStatusId == 3 || e.OrderStatusId == 6 || e.OrderStatusId == 9) && e.Balance != 0).ToListAsync();

                foreach (var order in over60Days)
                {
                    var paymentTermName = string.Empty;

                    var paymentTerm = paymentTerms.FirstOrDefault(e => e.Id == order.PaymentTermId);

                    if (paymentTerm != null)
                    {
                        paymentTermName = paymentTerm.TermName;
                    }

                    //Check if customer is already in result
                    var record = result.FirstOrDefault(e => e.CustomerId == order.CustomerId);
                    if (record == null)
                    {
                        // Get last Payment of Customer
                        var payment = await _context.Payments.Where(e => e.CustomerId == order.CustomerId).OrderByDescending(o => o.Id).FirstOrDefaultAsync();

                        // Get last Customer Note of Customer
                        var customerNote = await _context.CustomerNotes.Where(e => e.CustomerId == order.CustomerId).OrderByDescending(o => o.Id).FirstOrDefaultAsync();

                        result.Add(new AgingBalanceReportDTO()
                        {
                            AccountNumber = order.AccountNumber,
                            CustomerId = order.CustomerId,
                            CustomerName = order.CustomerName,
                            PhoneNumber = order.PhoneNumber,
                            Email = order.Email != null ? order.Email : string.Empty,
                            State = order.BillState,
                            Current30Days = 0,
                            Over30Days = 0,
                            Over60Days = order.Balance,
                            Over90Days = 0,
                            TotalOwed = order.Balance,
                            LastPaymentDate = payment != null ? payment.PaymentDate : null,
                            PaymentTermName = paymentTermName,
                            LastUpdateDate = customerNote != null ? customerNote.CreatedDate : null
                        });
                    }
                    else
                    {
                        record.Over60Days += order.Balance;
                        record.TotalOwed += order.Balance;
                    }
                }

                // Get Over90Days
                var over90Days = await _context.Orders.Where(e => e.OrderDate.Date <= cutOffDate.AddDays(-91).Date &&
                e.IsActive && !e.IsDeleted && !e.IsQuote && !(e.OrderStatusId == 3 || e.OrderStatusId == 6 || e.OrderStatusId == 9) && e.Balance != 0).ToListAsync();

                foreach (var order in over90Days)
                {
                    var paymentTermName = string.Empty;

                    var paymentTerm = paymentTerms.FirstOrDefault(e => e.Id == order.PaymentTermId);

                    if (paymentTerm != null)
                    {
                        paymentTermName = paymentTerm.TermName;
                    }

                    //Check if customer is already in result
                    var record = result.FirstOrDefault(e => e.CustomerId == order.CustomerId);
                    if (record == null)
                    {
                        // Get last Payment of Customer
                        var payment = await _context.Payments.Where(e => e.CustomerId == order.CustomerId).OrderByDescending(o => o.Id).FirstOrDefaultAsync();

                        // Get last Customer Note of Customer
                        var customerNote = await _context.CustomerNotes.Where(e => e.CustomerId == order.CustomerId).OrderByDescending(o => o.Id).FirstOrDefaultAsync();

                        result.Add(new AgingBalanceReportDTO()
                        {
                            AccountNumber = order.AccountNumber,
                            CustomerId = order.CustomerId,
                            CustomerName = order.CustomerName,
                            PhoneNumber = order.PhoneNumber,
                            Email = order.Email != null ? order.Email : string.Empty,
                            State = order.BillState,
                            Current30Days = 0,
                            Over30Days = 0,
                            Over60Days = 0,
                            Over90Days = order.Balance,
                            TotalOwed = order.Balance,
                            LastPaymentDate = payment != null ? payment.PaymentDate : null,
                            PaymentTermName = paymentTermName,
                            LastUpdateDate = customerNote != null ? customerNote.CreatedDate : null
                        });
                    }
                    else
                    {
                        record.Over90Days += order.Balance;
                        record.TotalOwed += order.Balance;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return result.OrderByDescending(e => e.TotalOwed).ToList();
        }
        public async Task<List<StatementReportDTO>> GetStatementReport(DateTime reportDate, int paymentTermId, List<int> customerIds)
        {
            List<StatementReportDTO> result = new List<StatementReportDTO>();
            List<PaymentTerm> paymentTerms = await _context.PaymentTerms.ToListAsync();
            List<Customer> customerList = new List<Customer>();

            if (customerIds.Count > 0)
            {
                customerList = await _context.Customers.Where(e => customerIds.Contains(e.Id) && (paymentTermId != 0 ? e.PaymentTermId == paymentTermId : true) && e.IsActive && !e.IsDeleted).ToListAsync();
            }
            else //if (customerIds == null )
            {
                customerList = await _context.Customers.Where(e => (paymentTermId != 0 ? e.PaymentTermId == paymentTermId : true) && e.IsActive && !e.IsDeleted).ToListAsync();
            }

            await ProcessCustomerStatements(reportDate, paymentTermId, result, paymentTerms, customerList);
            return result;
        }
        public async Task<List<StatementTotalReportDTO>> GetStatementTotalReport(DateTime reportDate, int paymentTermId, List<int> customerIds)
        {
            List<StatementTotalReportDTO> result = new List<StatementTotalReportDTO>();
            List<PaymentTerm> paymentTerms = await _context.PaymentTerms.ToListAsync();
            List<Customer> customerList = new List<Customer>();

            if (customerIds.Count > 0)
            {
                customerList = await _context.Customers.Where(e => customerIds.Contains(e.Id) && (paymentTermId != 0 ? e.PaymentTermId == paymentTermId : true) && e.IsActive && !e.IsDeleted).ToListAsync();
            }
            else //if (customerIds == null )
            {
                customerList = await _context.Customers.Where(e => (paymentTermId != 0 ? e.PaymentTermId == paymentTermId : true) && e.IsActive && !e.IsDeleted).ToListAsync();
            }

            await ProcessCustomerStatementTotals(reportDate, paymentTermId, result, paymentTerms, customerList);

            return result;
        }
        private async Task ProcessCustomerStatements(DateTime reportDate, int paymentTermId, List<StatementReportDTO> result, List<PaymentTerm> paymentTerms, List<Customer> customerList)
        {
            foreach (var customer in customerList.OrderBy(e => e.CustomerName))
            {
                var orders = await _context.Orders.Where(e => e.OrderStatusId != 1 && e.CustomerId == customer.Id && !e.IsQuote && !(e.OrderStatusId == 3 || e.OrderStatusId == 6 || e.OrderStatusId == 9) &&
                        e.OrderDate < reportDate.AddDays(1) && (paymentTermId != 0 ? e.PaymentTermId == paymentTermId : true) &&
                         e.Balance != 0 && e.IsActive && !e.IsDeleted).OrderBy(o => o.CreatedDate).ToListAsync();

                if (orders.Count > 0)
                {
                    decimal totalDue = 0;
                    var paymentTerm = paymentTerms.Where(e => e.Id == customer.PaymentTermId).FirstOrDefault();

                    List<StatementReportOrderDTO> orderList = new List<StatementReportOrderDTO>();

                    foreach (var order in orders)
                    {
                        var ord = new StatementReportOrderDTO
                        {
                            Balance = order.Balance,
                            InvoiceNumber = order.InvoiceNumber,
                            OrderDate = order.OrderDate,
                            OrderId = order.Id,
                            OrderNumber = order.OrderNumber,
                            PurchaseOrderNumber = order.PurchaseOrderNumber,
                            TotalAmount = order.TotalAmount != null ? order.TotalAmount : 0,
                            OrderType = order.OrderStatusId == 5 ? "Credit" : "Order"
                        };

                        orderList.Add(ord);
                    }

                    foreach (var order in orderList)
                    {
                        totalDue += order.Balance != null ? (decimal)order.Balance : 0;
                    }

                    var statement = new StatementReportDTO
                    {
                        AccountNumber = customer.AccountNumber,
                        AddressLine1 = customer.AddressLine1,
                        AddressLine2 = customer.AddressLine2 != null ? customer.AddressLine2 : "",
                        City = customer.City,
                        Country = customer.Country,
                        CustomerId = customer.Id,
                        CustomerName = customer.CustomerName,
                        PaymentTermId = paymentTerm != null ? paymentTerm.Id : 0,
                        PaymentTermName = paymentTerm != null ? paymentTerm.TermName : "",
                        PhoneNumber = customer.PhoneNumber,
                        ZipCode = customer.ZipCode,
                        State = customer.State,
                        TotalDue = totalDue,
                        Orders = orderList,
                    };

                    result.Add(statement);
                }
            }
        }
        private async Task ProcessCustomerStatementTotals(DateTime reportDate, int paymentTermId, List<StatementTotalReportDTO> result, List<PaymentTerm> paymentTerms, List<Customer> customerList)
        {
            foreach (var customer in customerList.OrderBy(e => e.CustomerName))
            {
                var orders = await _context.Orders.Where(e => e.OrderStatusId != 1 && e.CustomerId == customer.Id && !e.IsQuote && !(e.OrderStatusId == 3 || e.OrderStatusId == 6 || e.OrderStatusId == 9) &&
                        e.OrderDate.Date <= reportDate.Date && (paymentTermId != 0 ? e.PaymentTermId == paymentTermId : true) &&
                         e.Balance != 0 && e.IsActive && !e.IsDeleted).OrderBy(o => o.CreatedDate) .ToListAsync();

                if (orders.Count > 0)
                {
                    decimal totalDue = 0;
                    var paymentTerm = paymentTerms.Where(e => e.Id == customer.PaymentTermId).FirstOrDefault();

                    foreach (var order in orders)
                    {
                        totalDue += order.Balance != null ? (decimal)order.Balance : 0;
                    }

                    var statement = new StatementTotalReportDTO
                    {
                        AccountNumber = customer.AccountNumber,
                        CustomerId = customer.Id,
                        TotalDue = totalDue
                    };

                    result.Add(statement);
                }
            }
        }
        public async Task<List<Order>> GetDiscountsByInvoiceNumber(int invoiceNumber, List<string> partNumbers)
        {
            var result = await (
                from order in _context.Orders
                join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                where order.IsActive && !order.IsDeleted && !order.IsQuote && order.OrderStatusId == 5 && 
                    (order.OriginalInvoiceNumber != null && order.OriginalInvoiceNumber == invoiceNumber) &&
                    (partNumbers.Contains(orderDetail.PartNumber))
                select order)
            .Distinct()
            .OrderBy(e => e.OrderNumber)
            .ToListAsync();

            foreach (var order in result)
            {
                order.Customer = await _context.Customers.Where(e => e.Id == order.CustomerId).FirstOrDefaultAsync();
                order.CustomerName = order.Customer != null ? order.Customer.CustomerName : "";
                order.AccountNumber = order.Customer != null ? order.Customer.AccountNumber : 0;

                var priceLevel = new PriceLevel();

                if (order.Customer != null)
                {
                    priceLevel = await _context.PriceLevels.Where(e => e.Id == order.Customer.PriceLevelId).FirstOrDefaultAsync();
                }

                order.PriceLevelName = priceLevel != null ? priceLevel.LevelName : "";

                order.PaymentTerm = await _context.PaymentTerms.Where(e => e.Id == order.PaymentTermId).FirstOrDefaultAsync();
                order.PaymentTermName = order.PaymentTerm != null ? order.PaymentTerm.TermName : "";

                order.Warehouse = await _context.Warehouses.Where(e => e.Id == order.WarehouseId).FirstOrDefaultAsync();
                order.WarehouseName = order.Warehouse != null ? order.Warehouse.WarehouseName : "";

                order.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id && e.IsActive && !e.IsDeleted).OrderBy(od => od.PartNumber).ToListAsync();

                foreach (var detail in order.OrderDetails)
                {
                    detail.VendorCatalogs = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == detail.MainPartsLinkNumber.Trim()).ToListAsync();
                }
            }

            return result;
        }
        public async Task<List<CustomerSalesAmountDTO>> GetCustomerSales(CustomerSalesFilterDTO filter)
        {
            var result = new List<CustomerSalesAmountDTO>();

            try
            {
                // US Settings
                //DateTime frDate = DateTime.Parse("2024-03-11 08:00:00");
                //DateTime toDate = frDate.AddDays(1);

                DateTime? frDate; // = currentDate; //new DateTime(dt.Year, dt.Month, dt.Day, 8, 00, 00);
                DateTime? toDate; // = currentDate.AddDays(1); //new DateTime(dt.Year, dt.Month, dt.Day + 1, 8, 00, 00);

                #region Column 1
                frDate = filter.Col1FrDate;
                toDate = filter.Col1ToDate;
                if (frDate != null && toDate != null)
                {
                    toDate = toDate.Value.AddDays(1);
                    var orders = await _context.Orders.Where(e => (e.OrderDate >= frDate && e.OrderDate < toDate) && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                                 !(e.OrderStatusId == 3 || e.OrderStatusId == 9)).OrderBy(o => o.CustomerName).ToListAsync();

                    if (orders.Any())
                    {
                        foreach (var order in orders)
                        {
                            var customer = result.FirstOrDefault(c => c.CustomerId == order.CustomerId);

                            if (customer == null && order.TotalAmount != 0)
                            {
                                CreateNewCustomerSales(result, order);
                            }
                            else
                            {
                                if (customer != null)
                                {
                                    UpdateCustomerSales(order, customer);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Column 2
                frDate = filter.Col2FrDate;
                toDate = filter.Col2ToDate;
                if (frDate != null && toDate != null)
                {
                    toDate = toDate.Value.AddDays(1);
                    var orders = await _context.Orders.Where(e => (e.OrderDate >= frDate && e.OrderDate < toDate) && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                                 !(e.OrderStatusId == 3 || e.OrderStatusId == 9)).OrderBy(o => o.CustomerName).ToListAsync();

                    if (orders.Any())
                    {
                        foreach (var order in orders)
                        {
                            var customer = result.FirstOrDefault(c => c.CustomerId == order.CustomerId);

                            if (customer == null && order.TotalAmount != 0)
                            {
                                result.Add(
                                    new CustomerSalesAmountDTO()
                                    {
                                        CustomerId = order.CustomerId,
                                        CustomerName = order.CustomerName,
                                        AccountNumber = order.AccountNumber,
                                        PhoneNumber = order.ShipPhoneNumber,
                                        State = order.ShipState,
                                        Column2 = new SalesData()
                                        {
                                            Amount = order.TotalAmount != null ? order.TotalAmount : 0,
                                            Cost = order.CurrentCost != null ? order.CurrentCost : 0,
                                            //Profit = order.TotalAmount - order.CurrentCost
                                        }
                                    });
                            }
                            else
                            {
                                if (customer != null)
                                {
                                    if (customer.Column2.Amount == null) customer.Column2.Amount = 0;
                                    if (customer.Column2.Cost == null) customer.Column2.Cost = 0;
                                    //if (customer.Column2.Profit == null) customer.Column2.Profit = 0;

                                    customer.Column2.Amount += order.TotalAmount != null ? order.TotalAmount : 0;
                                    customer.Column2.Cost += order.CurrentCost != null ? order.CurrentCost : 0;
                                    //customer.Column2.Profit += order.TotalAmount - order.CurrentCost != null ? order.CurrentCost : 0;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Column 3
                frDate = filter.Col3FrDate;
                toDate = filter.Col3ToDate;
                if (frDate != null && toDate != null)
                {
                    toDate = toDate.Value.AddDays(1);
                    var orders = await _context.Orders.Where(e => (e.OrderDate >= frDate && e.OrderDate < toDate) && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                                 !(e.OrderStatusId == 3 || e.OrderStatusId == 9)).OrderBy(o => o.CustomerName).ToListAsync();

                    if (orders.Any())
                    {
                        foreach (var order in orders)
                        {
                            var customer = result.FirstOrDefault(c => c.CustomerId == order.CustomerId);

                            if (customer == null && order.TotalAmount != 0)
                            {
                                result.Add(
                                    new CustomerSalesAmountDTO()
                                    {
                                        CustomerId = order.CustomerId,
                                        CustomerName = order.CustomerName,
                                        AccountNumber = order.AccountNumber,
                                        PhoneNumber = order.ShipPhoneNumber,
                                        State = order.ShipState,
                                        Column3 = new SalesData()
                                        {
                                            Amount = order.TotalAmount != null ? order.TotalAmount : 0,
                                            Cost = order.CurrentCost != null ? order.CurrentCost : 0,
                                            //Profit = order.TotalAmount - order.CurrentCost
                                        }
                                    });
                            }
                            else
                            {
                                if (customer != null)
                                {
                                    if (customer.Column3.Amount == null) customer.Column3.Amount = 0;
                                    if (customer.Column3.Cost == null) customer.Column3.Cost = 0;
                                    //if (customer.Column3.Profit == null) customer.Column3.Profit = 0;

                                    customer.Column3.Amount += order.TotalAmount != null ? order.TotalAmount : 0;
                                    customer.Column3.Cost += order.CurrentCost != null ? order.CurrentCost : 0;
                                    //customer.Column3.Profit += order.TotalAmount - order.CurrentCost != null ? order.CurrentCost : 0;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Column 4
                frDate = filter.Col4FrDate;
                toDate = filter.Col4ToDate;
                if (frDate != null && toDate != null)
                {
                    toDate = toDate.Value.AddDays(1);
                    var orders = await _context.Orders.Where(e => (e.OrderDate >= frDate && e.OrderDate < toDate) && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                                 !(e.OrderStatusId == 3 || e.OrderStatusId == 9)).OrderBy(o => o.CustomerName).ToListAsync();

                    if (orders.Any())
                    {
                        foreach (var order in orders)
                        {
                            var customer = result.FirstOrDefault(c => c.CustomerId == order.CustomerId);

                            if (customer == null && order.TotalAmount != 0)
                            {
                                result.Add(
                                    new CustomerSalesAmountDTO()
                                    {
                                        CustomerId = order.CustomerId,
                                        CustomerName = order.CustomerName,
                                        AccountNumber = order.AccountNumber,
                                        PhoneNumber = order.ShipPhoneNumber,
                                        State = order.ShipState,
                                        Column4 = new SalesData()
                                        {
                                            Amount = order.TotalAmount != null ? order.TotalAmount : 0,
                                            Cost = order.CurrentCost != null ? order.CurrentCost : 0,
                                            //Profit = order.TotalAmount - order.CurrentCost
                                        }
                                    });
                            }
                            else
                            {
                                if (customer != null)
                                {
                                    if (customer.Column4.Amount == null) customer.Column4.Amount = 0;
                                    if (customer.Column4.Cost == null) customer.Column4.Cost = 0;
                                    //if (customer.Column4.Profit == null) customer.Column4.Profit = 0;

                                    customer.Column4.Amount += order.TotalAmount != null ? order.TotalAmount : 0;
                                    customer.Column4.Cost += order.CurrentCost != null ? order.CurrentCost : 0;
                                    //customer.Column4.Profit += order.TotalAmount - order.CurrentCost != null ? order.CurrentCost : 0;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Column 5
                frDate = filter.Col5FrDate;
                toDate = filter.Col5ToDate;
                if (frDate != null && toDate != null)
                {
                    toDate = toDate.Value.AddDays(1);
                    var orders = await _context.Orders.Where(e => (e.OrderDate >= frDate && e.OrderDate < toDate) && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                                 !(e.OrderStatusId == 3 || e.OrderStatusId == 9)).OrderBy(o => o.CustomerName).ToListAsync();

                    if (orders.Any())
                    {
                        foreach (var order in orders)
                        {
                            var customer = result.FirstOrDefault(c => c.CustomerId == order.CustomerId);

                            if (customer == null && order.TotalAmount != 0)
                            {
                                result.Add(
                                    new CustomerSalesAmountDTO()
                                    {
                                        CustomerId = order.CustomerId,
                                        CustomerName = order.CustomerName,
                                        AccountNumber = order.AccountNumber,
                                        PhoneNumber = order.ShipPhoneNumber,
                                        State = order.ShipState,
                                        Column5 = new SalesData()
                                        {
                                            Amount = order.TotalAmount != null ? order.TotalAmount : 0,
                                            Cost = order.CurrentCost != null ? order.CurrentCost : 0,
                                            //Profit = order.TotalAmount - order.CurrentCost
                                        }
                                    });
                            }
                            else
                            {
                                if (customer != null)
                                {
                                    if (customer.Column5.Amount == null) customer.Column5.Amount = 0;
                                    if (customer.Column5.Cost == null) customer.Column5.Cost = 0;
                                    //if (customer.Column5.Profit == null) customer.Column5.Profit = 0;

                                    customer.Column5.Amount += order.TotalAmount != null ? order.TotalAmount : 0;
                                    customer.Column5.Cost += order.CurrentCost != null ? order.CurrentCost : 0;
                                    //customer.Column5.Profit += order.TotalAmount - order.CurrentCost != null ? order.CurrentCost : 0;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Column 6
                frDate = filter.Col6FrDate;
                toDate = filter.Col6ToDate;
                if (frDate != null && toDate != null)
                {
                    toDate = toDate.Value.AddDays(1);
                    var orders = await _context.Orders.Where(e => (e.OrderDate >= frDate && e.OrderDate < toDate) && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                                 !(e.OrderStatusId == 3 || e.OrderStatusId == 9)).OrderBy(o => o.CustomerName).ToListAsync();

                    if (orders.Any())
                    {
                        foreach (var order in orders)
                        {
                            var customer = result.FirstOrDefault(c => c.CustomerId == order.CustomerId);

                            if (customer == null && order.TotalAmount != 0)
                            {
                                result.Add(
                                    new CustomerSalesAmountDTO()
                                    {
                                        CustomerId = order.CustomerId,
                                        CustomerName = order.CustomerName,
                                        AccountNumber = order.AccountNumber,
                                        PhoneNumber = order.ShipPhoneNumber,
                                        State = order.ShipState,
                                        Column6 = new SalesData()
                                        {
                                            Amount = order.TotalAmount != null ? order.TotalAmount : 0,
                                            Cost = order.CurrentCost != null ? order.CurrentCost : 0,
                                            //Profit = order.TotalAmount - order.CurrentCost
                                        }
                                    });
                            }
                            else
                            {
                                if (customer != null)
                                {
                                    if (customer.Column6.Amount == null) customer.Column6.Amount = 0;
                                    if (customer.Column6.Cost == null) customer.Column6.Cost = 0;
                                    //if (customer.Column6.Profit == null) customer.Column6.Profit = 0;

                                    customer.Column6.Amount += order.TotalAmount != null ? order.TotalAmount : 0;
                                    customer.Column6.Cost += order.CurrentCost != null ? order.CurrentCost : 0;
                                    //customer.Column6.Profit += order.TotalAmount - order.CurrentCost != null ? order.CurrentCost : 0;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Column 7
                frDate = filter.Col7FrDate;
                toDate = filter.Col7ToDate;
                if (frDate != null && toDate != null)
                {
                    toDate = toDate.Value.AddDays(1);
                    var orders = await _context.Orders.Where(e => (e.OrderDate >= frDate && e.OrderDate < toDate) && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                                 !(e.OrderStatusId == 3 || e.OrderStatusId == 9)).OrderBy(o => o.CustomerName).ToListAsync();

                    if (orders.Any())
                    {
                        foreach (var order in orders)
                        {
                            var customer = result.FirstOrDefault(c => c.CustomerId == order.CustomerId);

                            if (customer == null && order.TotalAmount != 0)
                            {
                                result.Add(
                                    new CustomerSalesAmountDTO()
                                    {
                                        CustomerId = order.CustomerId,
                                        CustomerName = order.CustomerName,
                                        AccountNumber = order.AccountNumber,
                                        PhoneNumber = order.ShipPhoneNumber,
                                        State = order.ShipState,
                                        Column7 = new SalesData()
                                        {
                                            Amount = order.TotalAmount != null ? order.TotalAmount : 0,
                                            Cost = order.CurrentCost != null ? order.CurrentCost : 0,
                                            //Profit = order.TotalAmount - order.CurrentCost
                                        }
                                    });
                            }
                            else
                            {
                                if (customer != null)
                                {
                                    if (customer.Column7.Amount == null) customer.Column7.Amount = 0;
                                    if (customer.Column7.Cost == null) customer.Column7.Cost = 0;
                                    //if (customer.Column7.Profit == null) customer.Column7.Profit = 0;

                                    customer.Column7.Amount += order.TotalAmount != null ? order.TotalAmount : 0;
                                    customer.Column7.Cost += order.CurrentCost != null ? order.CurrentCost : 0;
                                    //customer.Column7.Profit += order.TotalAmount - order.CurrentCost != null ? order.CurrentCost : 0;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Column 8
                frDate = filter.Col8FrDate;
                toDate = filter.Col8ToDate;
                if (frDate != null && toDate != null)
                {
                    toDate = toDate.Value.AddDays(1);
                    var orders = await _context.Orders.Where(e => (e.OrderDate >= frDate && e.OrderDate < toDate) && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                                 !(e.OrderStatusId == 3 || e.OrderStatusId == 9)).OrderBy(o => o.CustomerName).ToListAsync();

                    if (orders.Any())
                    {
                        foreach (var order in orders)
                        {
                            var customer = result.FirstOrDefault(c => c.CustomerId == order.CustomerId);

                            if (customer == null && order.TotalAmount != 0)
                            {
                                result.Add(
                                    new CustomerSalesAmountDTO()
                                    {
                                        CustomerId = order.CustomerId,
                                        CustomerName = order.CustomerName,
                                        AccountNumber = order.AccountNumber,
                                        PhoneNumber = order.ShipPhoneNumber,
                                        State = order.ShipState,
                                        Column8 = new SalesData()
                                        {
                                            Amount = order.TotalAmount != null ? order.TotalAmount : 0,
                                            Cost = order.CurrentCost != null ? order.CurrentCost : 0,
                                            //Profit = order.TotalAmount - order.CurrentCost
                                        }
                                    });
                            }
                            else
                            {
                                if (customer != null)
                                {
                                    if (customer.Column8.Amount == null) customer.Column8.Amount = 0;
                                    if (customer.Column8.Cost == null) customer.Column8.Cost = 0;
                                    //if (customer.Column8.Profit == null) customer.Column8.Profit = 0;

                                    customer.Column8.Amount += order.TotalAmount != null ? order.TotalAmount : 0;
                                    customer.Column8.Cost += order.CurrentCost != null ? order.CurrentCost : 0;
                                    //customer.Column8.Profit += order.TotalAmount - order.CurrentCost != null ? order.CurrentCost : 0;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Column 9
                frDate = filter.Col9FrDate;
                toDate = filter.Col9ToDate;
                if (frDate != null && toDate != null)
                {
                    toDate = toDate.Value.AddDays(1);
                    var orders = await _context.Orders.Where(e => (e.OrderDate >= frDate && e.OrderDate < toDate) && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                                 !(e.OrderStatusId == 3 || e.OrderStatusId == 9)).OrderBy(o => o.CustomerName).ToListAsync();

                    if (orders.Any())
                    {
                        foreach (var order in orders)
                        {
                            var customer = result.FirstOrDefault(c => c.CustomerId == order.CustomerId);

                            if (customer == null && order.TotalAmount != 0)
                            {
                                result.Add(
                                    new CustomerSalesAmountDTO()
                                    {
                                        CustomerId = order.CustomerId,
                                        CustomerName = order.CustomerName,
                                        AccountNumber = order.AccountNumber,
                                        PhoneNumber = order.ShipPhoneNumber,
                                        State = order.ShipState,
                                        Column9 = new SalesData()
                                        {
                                            Amount = order.TotalAmount != null ? order.TotalAmount : 0,
                                            Cost = order.CurrentCost != null ? order.CurrentCost : 0,
                                            //Profit = order.TotalAmount - order.CurrentCost
                                        }
                                    });
                            }
                            else
                            {
                                if (customer != null)
                                {
                                    if (customer.Column9.Amount == null) customer.Column9.Amount = 0;
                                    if (customer.Column9.Cost == null) customer.Column9.Cost = 0;
                                    //if (customer.Column9.Profit == null) customer.Column9.Profit = 0;

                                    customer.Column9.Amount += order.TotalAmount != null ? order.TotalAmount : 0;
                                    customer.Column9.Cost += order.CurrentCost != null ? order.CurrentCost : 0;
                                    //customer.Column9.Profit += order.TotalAmount - order.CurrentCost != null ? order.CurrentCost : 0;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Column 10
                frDate = filter.Col10FrDate;
                toDate = filter.Col10ToDate;
                if (frDate != null && toDate != null)
                {
                    toDate = toDate.Value.AddDays(1);
                    var orders = await _context.Orders.Where(e => (e.OrderDate >= frDate && e.OrderDate < toDate) && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                                 !(e.OrderStatusId == 3 || e.OrderStatusId == 9)).OrderBy(o => o.CustomerName).ToListAsync();

                    if (orders.Any())
                    {
                        foreach (var order in orders)
                        {
                            var customer = result.FirstOrDefault(c => c.CustomerId == order.CustomerId);

                            if (customer == null && order.TotalAmount != 0)
                            {
                                result.Add(
                                    new CustomerSalesAmountDTO()
                                    {
                                        CustomerId = order.CustomerId,
                                        CustomerName = order.CustomerName,
                                        AccountNumber = order.AccountNumber,
                                        PhoneNumber = order.ShipPhoneNumber,
                                        State = order.ShipState,
                                        Column10 = new SalesData()
                                        {
                                            Amount = order.TotalAmount != null ? order.TotalAmount : 0,
                                            Cost = order.CurrentCost != null ? order.CurrentCost : 0,
                                            //Profit = order.TotalAmount - order.CurrentCost
                                        }
                                    });
                            }
                            else
                            {
                                if (customer != null)
                                {
                                    if (customer.Column10.Amount == null) customer.Column10.Amount = 0;
                                    if (customer.Column10.Cost == null) customer.Column10.Cost = 0;
                                    //if (customer.Column10.Profit == null) customer.Column10.Profit = 0;

                                    customer.Column10.Amount += order.TotalAmount != null ? order.TotalAmount : 0;
                                    customer.Column10.Cost += order.CurrentCost != null ? order.CurrentCost : 0;
                                    //customer.Column10.Profit += order.TotalAmount - order.CurrentCost != null ? order.CurrentCost : 0;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Column 11
                frDate = filter.Col11FrDate;
                toDate = filter.Col11ToDate;
                if (frDate != null && toDate != null)
                {
                    toDate = toDate.Value.AddDays(1);
                    var orders = await _context.Orders.Where(e => (e.OrderDate >= frDate && e.OrderDate < toDate) && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                                 !(e.OrderStatusId == 3 || e.OrderStatusId == 9)).OrderBy(o => o.CustomerName).ToListAsync();

                    if (orders.Any())
                    {
                        foreach (var order in orders)
                        {
                            var customer = result.FirstOrDefault(c => c.CustomerId == order.CustomerId);

                            if (customer == null && order.TotalAmount != 0)
                            {
                                result.Add(
                                    new CustomerSalesAmountDTO()
                                    {
                                        CustomerId = order.CustomerId,
                                        CustomerName = order.CustomerName,
                                        AccountNumber = order.AccountNumber,
                                        PhoneNumber = order.ShipPhoneNumber,
                                        State = order.ShipState,
                                        Column11 = new SalesData()
                                        {
                                            Amount = order.TotalAmount != null ? order.TotalAmount : 0,
                                            Cost = order.CurrentCost != null ? order.CurrentCost : 0,
                                            //Profit = order.TotalAmount - order.CurrentCost
                                        }
                                    });
                            }
                            else
                            {
                                if (customer != null)
                                {
                                    if (customer.Column11.Amount == null) customer.Column11.Amount = 0;
                                    if (customer.Column11.Cost == null) customer.Column11.Cost = 0;
                                    //if (customer.Column11.Profit == null) customer.Column11.Profit = 0;

                                    customer.Column11.Amount += order.TotalAmount != null ? order.TotalAmount : 0;
                                    customer.Column11.Cost += order.CurrentCost != null ? order.CurrentCost : 0;
                                    //customer.Column11.Profit += order.TotalAmount - order.CurrentCost != null ? order.CurrentCost : 0;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Column 12
                frDate = filter.Col12FrDate;
                toDate = filter.Col12ToDate;
                if (frDate != null && toDate != null)
                {
                    toDate = toDate.Value.AddDays(1);
                    var orders = await _context.Orders.Where(e => (e.OrderDate >= frDate && e.OrderDate < toDate) && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                                 !(e.OrderStatusId == 3 || e.OrderStatusId == 9)).OrderBy(o => o.CustomerName).ToListAsync();

                    if (orders.Any())
                    {
                        foreach (var order in orders)
                        {
                            var customer = result.FirstOrDefault(c => c.CustomerId == order.CustomerId);

                            if (customer == null && order.TotalAmount != 0)
                            {
                                result.Add(
                                    new CustomerSalesAmountDTO()
                                    {
                                        CustomerId = order.CustomerId,
                                        CustomerName = order.CustomerName,
                                        AccountNumber = order.AccountNumber,
                                        PhoneNumber = order.ShipPhoneNumber,
                                        State = order.ShipState,
                                        Column12 = new SalesData()
                                        {
                                            Amount = order.TotalAmount != null ? order.TotalAmount : 0,
                                            Cost = order.CurrentCost != null ? order.CurrentCost : 0,
                                            //Profit = order.TotalAmount - order.CurrentCost
                                        }
                                    });
                            }
                            else
                            {
                                if (customer != null)
                                {
                                    if (customer.Column12.Amount == null) customer.Column12.Amount = 0;
                                    if (customer.Column12.Cost == null) customer.Column12.Cost = 0;
                                    //if (customer.Column12.Profit == null) customer.Column12.Profit = 0;

                                    customer.Column12.Amount += order.TotalAmount != null ? order.TotalAmount : 0;
                                    customer.Column12.Cost += order.CurrentCost != null ? order.CurrentCost : 0;
                                    //customer.Column12.Profit += order.TotalAmount - order.CurrentCost != null ? order.CurrentCost : 0;
                                }
                            }
                        }
                    }
                }
                #endregion

                GetCustomerProfitAndMargin(result);

                var grandTotal = new CustomerSalesAmountDTO()
                {
                    AccountNumber = 0,
                    CustomerId = 0,
                    CustomerName = "GRAND TOTALS",
                    PhoneNumber = string.Empty,
                    State = string.Empty,
                    Column1 = new SalesData() { Amount = 0, Cost = 0, Profit = 0, Margin = 0 },
                    Column2 = new SalesData() { Amount = 0, Cost = 0, Profit = 0, Margin = 0 },
                    Column3 = new SalesData() { Amount = 0, Cost = 0, Profit = 0, Margin = 0 },
                    Column4 = new SalesData() { Amount = 0, Cost = 0, Profit = 0, Margin = 0 },
                    Column5 = new SalesData() { Amount = 0, Cost = 0, Profit = 0, Margin = 0 },
                    Column6 = new SalesData() { Amount = 0, Cost = 0, Profit = 0, Margin = 0 },
                    Column7 = new SalesData() { Amount = 0, Cost = 0, Profit = 0, Margin = 0 },
                    Column8 = new SalesData() { Amount = 0, Cost = 0, Profit = 0, Margin = 0 },
                    Column9 = new SalesData() { Amount = 0, Cost = 0, Profit = 0, Margin = 0 },
                    Column10 = new SalesData() { Amount = 0, Cost = 0, Profit = 0, Margin = 0 },
                    Column11 = new SalesData() { Amount = 0, Cost = 0, Profit = 0, Margin = 0 },
                    Column12 = new SalesData() { Amount = 0, Cost = 0, Profit = 0, Margin = 0 },
                };

                GetGrandTotals(result, grandTotal);

                result = result.OrderBy(c => c.CustomerName).ToList();
                result.Insert(0, grandTotal);

                return result;
            }
            catch (Exception ex)
            {
                return new List<CustomerSalesAmountDTO>();
                //return result;
            }
        }

        private static void UpdateCustomerSales(Order? order, CustomerSalesAmountDTO? customer)
        {
            if (customer.Column1.Amount == null) customer.Column1.Amount = 0;
            if (customer.Column1.Cost == null) customer.Column1.Cost = 0;
            //if (customer.Column1.Profit == null) customer.Column1.Profit = 0;

            customer.Column1.Amount += order.TotalAmount != null ? order.TotalAmount : 0;
            customer.Column1.Cost += order.CurrentCost != null ? order.CurrentCost : 0;
            //customer.Column1.Profit += order.TotalAmount - order.CurrentCost != null ? order.CurrentCost : 0;
        }

        private static void CreateNewCustomerSales(List<CustomerSalesAmountDTO> result, Order? order)
        {
            result.Add(
                new CustomerSalesAmountDTO()
                {
                    CustomerId = order.CustomerId,
                    CustomerName = order.CustomerName,
                    AccountNumber = order.AccountNumber,
                    PhoneNumber = order.ShipPhoneNumber,
                    State = order.ShipState,
                    Column1 = new SalesData()
                    {
                        Amount = order.TotalAmount != null ? order.TotalAmount : 0,
                        Cost = order.CurrentCost != null ? order.CurrentCost : 0,
                        //Profit = order.TotalAmount - order.CurrentCost
                    }
                });
        }

        private void GetGrandTotals(List<CustomerSalesAmountDTO> result, CustomerSalesAmountDTO grandTotal)
        {
            // Get Amount
            grandTotal.Column1.Amount = result.Sum(e => e.Column1.Amount);
            grandTotal.Column2.Amount = result.Sum(e => e.Column2.Amount);
            grandTotal.Column3.Amount = result.Sum(e => e.Column3.Amount);
            grandTotal.Column4.Amount = result.Sum(e => e.Column4.Amount);
            grandTotal.Column5.Amount = result.Sum(e => e.Column5.Amount);
            grandTotal.Column6.Amount = result.Sum(e => e.Column6.Amount);
            grandTotal.Column7.Amount = result.Sum(e => e.Column7.Amount);
            grandTotal.Column8.Amount = result.Sum(e => e.Column8.Amount);
            grandTotal.Column9.Amount = result.Sum(e => e.Column9.Amount);
            grandTotal.Column10.Amount = result.Sum(e => e.Column10.Amount);
            grandTotal.Column11.Amount = result.Sum(e => e.Column11.Amount);
            grandTotal.Column12.Amount = result.Sum(e => e.Column12.Amount);

            // Get Cost
            grandTotal.Column1.Cost = result.Sum(e => e.Column1.Cost);
            grandTotal.Column2.Cost = result.Sum(e => e.Column2.Cost);
            grandTotal.Column3.Cost = result.Sum(e => e.Column3.Cost);
            grandTotal.Column4.Cost = result.Sum(e => e.Column4.Cost);
            grandTotal.Column5.Cost = result.Sum(e => e.Column5.Cost);
            grandTotal.Column6.Cost = result.Sum(e => e.Column6.Cost);
            grandTotal.Column7.Cost = result.Sum(e => e.Column7.Cost);
            grandTotal.Column8.Cost = result.Sum(e => e.Column8.Cost);
            grandTotal.Column9.Cost = result.Sum(e => e.Column9.Cost);
            grandTotal.Column10.Cost = result.Sum(e => e.Column10.Cost);
            grandTotal.Column11.Cost = result.Sum(e => e.Column11.Cost);
            grandTotal.Column12.Cost = result.Sum(e => e.Column12.Cost);

            // Get Profit
            grandTotal.Column1.Profit = grandTotal.Column1.Amount - grandTotal.Column1.Cost;
            grandTotal.Column2.Profit = grandTotal.Column2.Amount - grandTotal.Column2.Cost;
            grandTotal.Column3.Profit = grandTotal.Column3.Amount - grandTotal.Column3.Cost;
            grandTotal.Column4.Profit = grandTotal.Column4.Amount - grandTotal.Column4.Cost;
            grandTotal.Column5.Profit = grandTotal.Column5.Amount - grandTotal.Column5.Cost;
            grandTotal.Column6.Profit = grandTotal.Column6.Amount - grandTotal.Column6.Cost;
            grandTotal.Column7.Profit = grandTotal.Column7.Amount - grandTotal.Column7.Cost;
            grandTotal.Column8.Profit = grandTotal.Column8.Amount - grandTotal.Column8.Cost;
            grandTotal.Column9.Profit = grandTotal.Column9.Amount - grandTotal.Column9.Cost;
            grandTotal.Column10.Profit = grandTotal.Column10.Amount - grandTotal.Column10.Cost;
            grandTotal.Column11.Profit = grandTotal.Column11.Amount - grandTotal.Column11.Cost;
            grandTotal.Column12.Profit = grandTotal.Column12.Amount - grandTotal.Column12.Cost;

            // Get Margin
            grandTotal.Column1.Margin = grandTotal.Column1.Amount != 0 ? (grandTotal.Column1.Profit / grandTotal.Column1.Amount * 100) : 0;
            grandTotal.Column2.Margin = grandTotal.Column2.Amount != 0 ? (grandTotal.Column2.Profit / grandTotal.Column2.Amount * 100) : 0;
            grandTotal.Column3.Margin = grandTotal.Column3.Amount != 0 ? (grandTotal.Column3.Profit / grandTotal.Column3.Amount * 100) : 0;
            grandTotal.Column4.Margin = grandTotal.Column4.Amount != 0 ? (grandTotal.Column4.Profit / grandTotal.Column4.Amount * 100) : 0;
            grandTotal.Column5.Margin = grandTotal.Column5.Amount != 0 ? (grandTotal.Column5.Profit / grandTotal.Column5.Amount * 100) : 0;
            grandTotal.Column6.Margin = grandTotal.Column6.Amount != 0 ? (grandTotal.Column6.Profit / grandTotal.Column6.Amount * 100) : 0;
            grandTotal.Column7.Margin = grandTotal.Column7.Amount != 0 ? (grandTotal.Column7.Profit / grandTotal.Column7.Amount * 100) : 0;
            grandTotal.Column8.Margin = grandTotal.Column8.Amount != 0 ? (grandTotal.Column8.Profit / grandTotal.Column8.Amount * 100) : 0;
            grandTotal.Column9.Margin = grandTotal.Column9.Amount != 0 ? (grandTotal.Column9.Profit / grandTotal.Column9.Amount * 100) : 0;
            grandTotal.Column10.Margin = grandTotal.Column10.Amount != 0 ? (grandTotal.Column10.Profit / grandTotal.Column10.Amount * 100) : 0;
            grandTotal.Column11.Margin = grandTotal.Column11.Amount != 0 ? (grandTotal.Column11.Profit / grandTotal.Column11.Amount * 100) : 0;
            grandTotal.Column12.Margin = grandTotal.Column12.Amount != 0 ? (grandTotal.Column12.Profit / grandTotal.Column12.Amount * 100) : 0;
        }

        private void GetCustomerProfitAndMargin(List<CustomerSalesAmountDTO> result)
        {
            foreach (var customer in result)
            {
                // Get Profit
                customer.Column1.Profit = customer.Column1.Amount - customer.Column1.Cost;
                customer.Column2.Profit = customer.Column2.Amount - customer.Column2.Cost;
                customer.Column3.Profit = customer.Column3.Amount - customer.Column3.Cost;
                customer.Column4.Profit = customer.Column4.Amount - customer.Column4.Cost;
                customer.Column5.Profit = customer.Column5.Amount - customer.Column5.Cost;
                customer.Column6.Profit = customer.Column6.Amount - customer.Column6.Cost;
                customer.Column7.Profit = customer.Column7.Amount - customer.Column7.Cost;
                customer.Column8.Profit = customer.Column8.Amount - customer.Column8.Cost;
                customer.Column9.Profit = customer.Column9.Amount - customer.Column9.Cost;
                customer.Column10.Profit = customer.Column10.Amount - customer.Column10.Cost;
                customer.Column11.Profit = customer.Column11.Amount - customer.Column11.Cost;
                customer.Column12.Profit = customer.Column12.Amount - customer.Column12.Cost;

                // Get Margin

                customer.Column1.Margin = customer.Column1.Amount != 0 ? (customer.Column1.Profit / customer.Column1.Amount * 100) : 0;
                customer.Column2.Margin = customer.Column2.Amount != 0 ? (customer.Column2.Profit / customer.Column2.Amount * 100) : 0;
                customer.Column3.Margin = customer.Column3.Amount != 0 ? (customer.Column3.Profit / customer.Column3.Amount * 100) : 0;
                customer.Column4.Margin = customer.Column4.Amount != 0 ? (customer.Column4.Profit / customer.Column4.Amount * 100) : 0;
                customer.Column5.Margin = customer.Column5.Amount != 0 ? (customer.Column5.Profit / customer.Column5.Amount * 100) : 0;
                customer.Column6.Margin = customer.Column6.Amount != 0 ? (customer.Column6.Profit / customer.Column6.Amount * 100) : 0;
                customer.Column7.Margin = customer.Column7.Amount != 0 ? (customer.Column7.Profit / customer.Column7.Amount * 100) : 0;
                customer.Column8.Margin = customer.Column8.Amount != 0 ? (customer.Column8.Profit / customer.Column8.Amount * 100) : 0;
                customer.Column9.Margin = customer.Column9.Amount != 0 ? (customer.Column9.Profit / customer.Column9.Amount * 100) : 0;
                customer.Column10.Margin = customer.Column10.Amount != 0 ? (customer.Column10.Profit / customer.Column10.Amount * 100) : 0;
                customer.Column11.Margin = customer.Column11.Amount != 0 ? (customer.Column11.Profit / customer.Column11.Amount * 100) : 0;
                customer.Column12.Margin = customer.Column12.Amount != 0 ? (customer.Column12.Profit / customer.Column12.Amount * 100) : 0;
            }
        }

        public async Task<DeliverySummaryDTO> GetDeliverySummary(DateTime currentDate)
        {
            var result = new DeliverySummaryDTO();
            DateTime deliveryDate = currentDate.DayOfWeek == DayOfWeek.Friday ? currentDate.AddDays(3) : currentDate.AddDays(1);
            DateTime frDate = deliveryDate;
            DateTime toDate = deliveryDate.AddDays(1);
            result.DeliveryDate = deliveryDate;

            #region California
            var orders = await _context.Orders.Where(e => (e.DeliveryDate >= frDate && e.DeliveryDate <= toDate)  && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                e.OrderStatusId != 3 && (e.RGAType == 0 || e.RGAType == 2) && e.ShipState.Trim() == "CA").ToListAsync();

            var uniqueCustomers = orders.GroupBy(o => o.CustomerId);

            result.DeliverySummaryCA = new DeliverySummaryCA()
            {
                NumberOfStops = uniqueCustomers.Count(),
            };

            var uniqueZones = orders.GroupBy(z => z.ShipZone);


            foreach (var zone in uniqueZones)
            {
                result.DeliverySummaryCA.ZoneSummaries.Add(new ZoneSummary()
                {
                    Zone = int.Parse(zone.Key),
                    NumberOfStops = GetNumberOfStops(zone.ToList()) //orders.Count(e => e.ShipZone == zone.Key)
                });
            }

            #endregion

            #region Nevada
            orders = await _context.Orders.Where(e => (e.DeliveryDate >= frDate && e.DeliveryDate <= toDate) && !e.IsQuote && e.IsActive && !e.IsDeleted &&
                e.OrderStatusId != 3 && (e.RGAType == 0 || e.RGAType == 2) && e.ShipState.Trim() == "NV").ToListAsync();

            uniqueCustomers = orders.GroupBy(o => o.CustomerId);

            result.DeliverySummaryNV = new DeliverySummaryNV()
            {
                NumberOfStops = uniqueCustomers.Count(),
            };

            uniqueZones = orders.GroupBy(z => z.ShipZone);


            foreach (var zone in uniqueZones)
            {
                result.DeliverySummaryNV.ZoneSummaries.Add(new ZoneSummary()
                {
                    Zone = int.Parse(zone.Key),
                    NumberOfStops = GetNumberOfStops(zone.ToList()) //orders.Count(e => e.ShipZone == zone.Key)
                });
            }

            #endregion

            return result;
        }

        private int GetNumberOfStops(List<Order> zone)
        {
            List<int> customerIds = new List<int>();

            foreach(var order in zone)
            {
                if (customerIds.IndexOf(order.CustomerId) == -1)
                {
                    customerIds.Add(order.CustomerId);
                }
            }

            return customerIds.Count();
        }
        #endregion

        #region Save Data
        public async Task<bool> VoidOrder(Order order)
        {
            var result = false;
            try
            {
                //var order = await _context.Orders.FindAsync(orderId);
                if (order != null)
                {
                    order.OrderStatusId = 3;
                    order.OrderStatusName = "Void";
                    _context.Orders.Update(order);
                    await _context.SaveChangesAsync();
                    result = true;
                }

                // Update Customer Current Balance
                var customer = await _context.Customers.FirstOrDefaultAsync(e => e.Id == order.CustomerId);
                if (customer != null)
                {
                    var currentBalance = GetStatementReport(DateTime.Now, customer.PaymentTermId != null ? customer.PaymentTermId.Value : 0, new List<int>() { customer.Id }).Result;
                    if (currentBalance != null && currentBalance.Any())
                    {
                        customer.OverBalance = currentBalance[0].TotalDue;
                        _context.Customers.Update(customer);
                        await _context.SaveEntitiesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            
            return result;
        }

        public async Task<bool> DeleteRGAOrder(Order order)
        {
            var result = false;
            try
            {
                order.IsDeleted = true;
                order.OrderStatusName = "Deleted";
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                result = true;

                // Update Original Order ReturnQuantity
                var originalOrder = await _context.Orders.FirstOrDefaultAsync(e => e.InvoiceNumber == order.OriginalInvoiceNumber);
                if (originalOrder != null)
                {
                    foreach (var rgaDetail in order.OrderDetails)
                    {
                        var origOrderDetail = await _context.OrderDetails.FirstOrDefaultAsync(e => e.OrderId == originalOrder.Id && e.ProductId == rgaDetail.ProductId);
                        if (origOrderDetail != null)
                        {
                            origOrderDetail.ReturnQuantity -= rgaDetail.OrderQuantity;

                            if (origOrderDetail.ReturnQuantity == 0)
                            {
                                origOrderDetail.StatusId = null;
                            }

                            _context.Orders.Update(order);
                        }
                    }

                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        public async Task<Order> Create(Order order)
        {
            try
            {
                List<WarehouseTracking> warehouseTrackingList = new List<WarehouseTracking>();
                var backOrderedList = new List<UserNotificationEmailDTO>();

                if (!order.IsQuote)
                {
                    var orderExist = await _context.Orders.FirstOrDefaultAsync(e => e.OrderNumber == order.OrderNumber);
                    if (orderExist != null)
                    {
                        var maxOrderNumber = await _context.Orders.MaxAsync(e => e.OrderNumber) + 1;
                        order.OrderNumber = maxOrderNumber;
                        var maxInvoiceNumber = await _context.Orders.MaxAsync(e => e.InvoiceNumber) + 1;
                        order.InvoiceNumber = maxInvoiceNumber;
                    }
                }
                else
                {
                    var quoteExist = await _context.Orders.FirstOrDefaultAsync(e => e.QuoteNumber == order.QuoteNumber);
                    if (quoteExist != null)
                    {
                        var maxQuoteNumber = await _context.Orders.MaxAsync(e => e.QuoteNumber) + 1;
                        order.QuoteNumber = maxQuoteNumber;
                    }
                }

                await _context.Orders.AddAsync(order);
                await _context.SaveEntitiesAsync();

                var orderDetailTracking = new Dictionary<string, string>();

                //var vendorCatalogs = new List<VendorCatalog>();
                foreach (var orderDetail in order.OrderDetails)
                {
                    orderDetail.OrderId = order.Id;

                    var tracking = !string.IsNullOrEmpty(orderDetail.WarehouseTracking) ? orderDetail.WarehouseTracking : "";

                    if (tracking.Contains("Original Order Number") && tracking.Contains("Blank"))
                    {
                        orderDetail.WarehouseTracking = string.Empty;
                    }

                    orderDetailTracking.Add(orderDetail.PartNumber, tracking);

                    //orderDetail.CreatedBy = order.CreatedBy;
                    //orderDetail.CreatedDate = order.CreatedDate;

                    //// Save Vendor Catalogs if not exist
                    //var existing = _context.VendorCatalogs.Any(e => e.PartsLinkNumber == orderDetail.MainPartsLinkNumber &&
                    //                                                e.VendorCode == orderDetail.VendorCode &&
                    //                                                e.VendorPartNumber == orderDetail.VendorPartNumber);
                    //if (!existing)
                    //{
                    //    var vendorCatalog = new VendorCatalog()
                    //    {
                    //        CreatedBy = orderDetail.CreatedBy,
                    //        CreatedDate = orderDetail.CreatedDate,
                    //        IsActive = orderDetail.IsActive,
                    //        IsDeleted = orderDetail.IsDeleted,
                    //        OnHand = orderDetail.VendorOnHand,
                    //        PartsLinkNumber = orderDetail.MainPartsLinkNumber,
                    //        Price = orderDetail.VendorPrice,
                    //        VendorCode = orderDetail.VendorCode,
                    //        VendorPartNumber = orderDetail.VendorPartNumber
                    //    };

                    //    vendorCatalogs.Add(vendorCatalog);
                    //}
                }

                await _context.OrderDetails.AddRangeAsync(order.OrderDetails);
                await _context.SaveEntitiesAsync();



                //await _context.VendorCatalogs.AddRangeAsync(vendorCatalogs);
                //await _context.SaveEntitiesAsync();


                foreach (var orderDetail in order.OrderDetails)
                {
                    //Get Warehouse Stocks with Location Name
                    var stocks = (
                        from stock in _context.WarehouseStocks
                        join product in _context.Products on stock.ProductId equals product.Id
                        join location in _context.WarehouseLocations on stock.WarehouseLocationId equals location.Id
                        where stock.ProductId == orderDetail.ProductId
                        select new WarehouseStock
                        {
                            Id = stock.Id,
                            CreatedBy = stock.CreatedBy,
                            CreatedDate = stock.CreatedDate,
                            IsActive = stock.IsActive,
                            IsDeleted = stock.IsDeleted,
                            Location = location.Location,
                            ModifiedBy = stock.ModifiedBy,
                            ModifiedDate = stock.ModifiedDate,
                            ProductId = orderDetail.ProductId,
                            Quantity = stock.Quantity > 0 ? stock.Quantity : 0,
                            WarehouseId = stock.WarehouseId,
                            WarehouseLocationId = stock.WarehouseLocationId,
                            CurrentCost = product.CurrentCost.HasValue ? product.CurrentCost.Value : 0
                        }).ToList();

                    orderDetail.Stocks = stocks;

                    //Add Warehouse Tracking
                    var tracking = orderDetailTracking[orderDetail.PartNumber];

                    if (!string.IsNullOrWhiteSpace(tracking))
                    {
                        //Add Order.WarehouseTrackings for Back Order
                        if (tracking.Contains("Original Order Number"))
                        {
                            var origOrderTracking = new WarehouseTracking();

                            // Get Warehouse Tracking or Original Order
                            var idxOrdNoStart = tracking.IndexOf("(") + 1;
                            var idxOrdNoEnd = tracking.IndexOf(")");
                            var idxOrdDetIdStart = tracking.IndexOf("[") + 1;
                            var idxOrdDetIdEnd = tracking.IndexOf("]");

                            var origOrdNo = tracking.Substring(idxOrdNoStart, idxOrdNoEnd - idxOrdNoStart);
                            var origOrdDetId = tracking.Substring(idxOrdDetIdStart, idxOrdDetIdEnd - idxOrdDetIdStart);

                            if (!string.IsNullOrEmpty(origOrdNo))
                            {
                                var origOrd = await _context.Orders.FirstOrDefaultAsync(e => e.OrderNumber == int.Parse(origOrdNo));
                                var origOrdDet = await _context.OrderDetails.FirstOrDefaultAsync(e => e.Id == int.Parse(origOrdDetId));
                                if (origOrd != null && origOrdDet != null)
                                {
                                    var origWareTrack = await _context.WarehouseTrackings.Where(e => e.OrderId == origOrd.Id && e.OrderDetailId == origOrdDet.Id).ToListAsync();
                                    if (origWareTrack.Any())
                                    {
                                        foreach (var track in  origWareTrack)
                                        {
                                            warehouseTrackingList.Add(
                                            new WarehouseTracking()
                                            {
                                                CreatedBy = track.CreatedBy,
                                                CreatedDate = track.CreatedDate.ToUniversalTime(),
                                                Description = track.Description,
                                                Id = 0,
                                                IsActive = true,
                                                IsDeleted = false,
                                                OrderDetailId = orderDetail.Id,
                                                OrderId = orderDetail.OrderId,
                                                Status = track.Status
                                            });

                                            if (track.Status == "Back Ordered")
                                            {
                                                origOrderTracking.CreatedBy = track.CreatedBy;
                                                origOrderTracking.CreatedDate = track.CreatedDate.ToUniversalTime();
                                                origOrderTracking.Description = $"Original Order Number {origOrdNo}";
                                                origOrderTracking.Id = 0;
                                                origOrderTracking.IsActive = true;
                                                origOrderTracking.IsDeleted = false;
                                                origOrderTracking.OrderDetailId = orderDetail.Id;
                                                origOrderTracking.OrderId = orderDetail.OrderId;
                                                origOrderTracking.Status = $"Original Order Number {origOrdNo}";
                                            }
                                        }
                                    }

                                    warehouseTrackingList.Add(origOrderTracking);

                                    // User Notification Email
                                    var user = await _context.Users.FirstOrDefaultAsync(e => e.UserName == origOrdDet.CreatedBy);
                                    if (user != null && !string.IsNullOrEmpty(user.Email))
                                    {
                                        backOrderedList.Add(new UserNotificationEmailDTO()
                                        {
                                            OrderNumber = origOrd.OrderNumber != null ? origOrd.OrderNumber.Value : 0,
                                            PartNumber = origOrdDet.PartNumber,
                                            Subject = "BackOrdered",
                                            Username = origOrdDet.CreatedBy,
                                            Email = user.Email
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            warehouseTrackingList.Add(
                            new WarehouseTracking()
                            {
                                CreatedBy = order.CreatedBy,
                                CreatedDate = order.CreatedDate.ToUniversalTime(),
                                Description = orderDetail.WarehouseTracking,
                                Id = 0,
                                IsActive = true,
                                IsDeleted = false,
                                OrderDetailId = orderDetail.Id,
                                OrderId = orderDetail.OrderId,
                                Status = orderDetail.WarehouseTracking
                            });
                        }
                    }
                }

                if (warehouseTrackingList.Count > 0)
                {
                    await _context.WarehouseTrackings.AddRangeAsync(warehouseTrackingList);
                    await _context.SaveEntitiesAsync();
                }

                if (backOrderedList.Any())
                {
                    _emailService.SendUserNotificationEmail(backOrderedList);
                }

                // Turn Off Customer Bypass Credit Limit
                var customer = await _context.Customers.FirstOrDefaultAsync(e => e.Id == order.CustomerId);
                if (customer != null)
                {
                    var currentBalance = GetStatementReport(DateTime.Now, customer.PaymentTermId != null ? customer.PaymentTermId.Value : 0, new List<int>() { customer.Id }).Result;
                    if (currentBalance != null && currentBalance.Any())
                    {
                        customer.OverBalance = currentBalance[0].TotalDue;
                    }

                    customer.IsBypassCreditLimit = false;
                    _context.Customers.Update(customer);
                    await _context.SaveEntitiesAsync();
                }

                return order;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //return false;
            }
            
        }

        public async Task<bool> CreateCreditMemo(Order order)
        {
            try
            {
                List<WarehouseTracking> warehouseTrackingList = new List<WarehouseTracking>();

                await _context.Orders.AddAsync(order);
                await _context.SaveEntitiesAsync();

                foreach (var orderDetail in order.OrderDetails)
                {
                    orderDetail.OrderId = order.Id;
                };

                await _context.OrderDetails.AddRangeAsync(order.OrderDetails);
                await _context.SaveEntitiesAsync();

                // Update Original Order and Order Detail Status
                var origOrder = await _context.Orders.Where(e => e.InvoiceNumber == order.OriginalInvoiceNumber).FirstOrDefaultAsync();
                if (origOrder != null)
                {
                    if (origOrder.OrderStatusId == 1)
                    {
                        origOrder.OrderStatusId = 2;
                        origOrder.OrderStatusName = "Posted";
                        _context.Orders.Update(origOrder);
                    }

                    var origOrderDetails = await _context.OrderDetails.Where(e => e.OrderId == origOrder.Id && e.IsActive && !e.IsDeleted).ToListAsync();
                    if (origOrderDetails.Count > 0)
                    {
                        foreach (var orderDetail in order.OrderDetails)
                        {
                            var od = origOrderDetails.Where(e => e.ProductId == orderDetail.ProductId).FirstOrDefault();
                            if (od != null)
                            {
                                od.StatusId = 5;
                                _context.OrderDetails.Update(od);

                                // Add Warehouse Tracking
                                warehouseTrackingList.Add(
                                    new WarehouseTracking()
                                    {
                                        CreatedBy = order.CreatedBy,
                                        CreatedDate = order.CreatedDate.ToUniversalTime(),
                                        Description = $"Credit Memo Created - {order.OrderNumber}",
                                        IsActive = true,
                                        IsDeleted = false,
                                        OrderDetailId = od.Id,
                                        OrderId = od.OrderId,
                                        Status = $"Credit Memo Created - {orderDetail.PartNumber}"
                                    });
                            }
                        }

                        _context.WarehouseTrackings.UpdateRange(warehouseTrackingList);
                        await _context.SaveEntitiesAsync();
                    }
                }

                // Update Customer Current Balance
                var customer = await _context.Customers.FirstOrDefaultAsync(e => e.Id == order.CustomerId);
                if (customer != null)
                {
                    var currentBalance = GetStatementReport(DateTime.Now, customer.PaymentTermId != null ? customer.PaymentTermId.Value : 0, new List<int>() { customer.Id }).Result;
                    if (currentBalance != null && currentBalance.Any())
                    {
                        customer.OverBalance = currentBalance[0].TotalDue;
                        _context.Customers.Update(customer);
                        await _context.SaveEntitiesAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateRGA(Order order)
        {
            try
            {
                List<WarehouseTracking> warehouseTrackingList = new List<WarehouseTracking>();

                await _context.Orders.AddAsync(order);
                await _context.SaveEntitiesAsync();

                foreach (var orderDetail in order.OrderDetails)
                {
                    orderDetail.OrderId = order.Id;
                };

                await _context.OrderDetails.AddRangeAsync(order.OrderDetails);
                await _context.SaveEntitiesAsync();

                // Update Original Order and Order Detail Status
                var origOrder = await _context.Orders.Where(e => e.InvoiceNumber == order.OriginalInvoiceNumber).FirstOrDefaultAsync();
                if (origOrder != null)
                {
                    if (origOrder.OrderStatusId == 1)
                    {
                        origOrder.OrderStatusId = 2;
                        origOrder.OrderStatusName = "Posted";
                        _context.Orders.Update(origOrder);
                    }

                    var origOrderDetails = await _context.OrderDetails.Where(e => e.OrderId == origOrder.Id && e.IsActive && !e.IsDeleted).ToListAsync();
                    if (origOrderDetails.Count > 0)
                    {
                        foreach (var orderDetail in order.OrderDetails)
                        {
                            var od = origOrderDetails.Where(e => e.ProductId == orderDetail.ProductId).FirstOrDefault();
                            if (od != null)
                            {
                                // Get RGA Order Detail
                                var rgaOd = order.OrderDetails.FirstOrDefault(od => od.ProductId == orderDetail.ProductId);
                                if (rgaOd != null)
                                {
                                    if (od.ReturnQuantity != null)
                                    {
                                        od.ReturnQuantity += rgaOd.OrderQuantity;
                                    }
                                    else
                                    {
                                        od.ReturnQuantity = rgaOd.OrderQuantity;
                                    }
                                    
                                    od.StatusId = 9;
                                    _context.OrderDetails.Update(od);

                                    // Add Warehouse Tracking
                                    warehouseTrackingList.Add(
                                        new WarehouseTracking()
                                        {
                                            CreatedBy = order.CreatedBy,
                                            CreatedDate = order.CreatedDate.ToUniversalTime(),
                                            Description = $"RGA Created - {order.OrderNumber}",
                                            IsActive = true,
                                            IsDeleted = false,
                                            OrderDetailId = od.Id,
                                            OrderId = od.OrderId,
                                            Status = $"RGA Created - {order.OrderNumber}"
                                        });
                                }
                            }
                        }

                        _context.WarehouseTrackings.UpdateRange(warehouseTrackingList);
                        await _context.SaveEntitiesAsync();
                    }
                }

                // Update Customer Current Balance
                var customer = await _context.Customers.FirstOrDefaultAsync(e => e.Id == order.CustomerId);
                if (customer != null)
                {
                    var currentBalance = GetStatementReport(DateTime.Now, customer.PaymentTermId != null ? customer.PaymentTermId.Value : 0, new List<int>() { customer.Id }).Result;
                    if (currentBalance != null && currentBalance.Any())
                    {
                        customer.OverBalance = currentBalance[0].TotalDue;
                        _context.Customers.Update(customer);
                        await _context.SaveEntitiesAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateDiscount(Order order)
        {
            try
            {
                await _context.Orders.AddAsync(order);
                await _context.SaveEntitiesAsync();

                foreach (var orderDetail in order.OrderDetails)
                {
                    orderDetail.OrderId = order.Id;
                };

                await _context.OrderDetails.AddRangeAsync(order.OrderDetails);
                await _context.SaveEntitiesAsync();

                // Update Original Order and Order Detail Status
                var origOrder = await _context.Orders.Where(e => e.InvoiceNumber == order.OriginalInvoiceNumber).FirstOrDefaultAsync();
                if (origOrder != null)
                {
                    if (origOrder.OrderStatusId == 1)
                    {
                        origOrder.OrderStatusId = 2;
                        origOrder.OrderStatusName = "Posted";
                        _context.Orders.Update(origOrder);
                    }

                    var origOrderDetails = await _context.OrderDetails.Where(e => e.OrderId == origOrder.Id && e.IsActive && !e.IsDeleted).ToListAsync();
                    if (origOrderDetails.Count > 0)
                    {
                        foreach (var orderDetail in order.OrderDetails)
                        {
                            var od = origOrderDetails.Where(e => e.ProductId == orderDetail.ProductId).FirstOrDefault();
                            if (od != null)
                            {
                                od.StatusId = 8;
                                _context.OrderDetails.Update(od);
                            }
                        }

                        await _context.SaveEntitiesAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateOverpayment(OverpaymentParameterDTO param)
        {
            var result = false;
            var amount = param.Amount * -1;

            try
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(e => e.Id == param.CustomerId);
                var product = await _context.Products.FirstOrDefaultAsync(e => e.PartNumber.Trim().ToLower() == "overpayment");
                var paymentTerms = await _context.PaymentTerms.ToListAsync();
                if (customer != null)
                {
                    var currentDate = DateTime.Now;
                    var maxOrderNumber = _context.Orders.Count() > 0 ? await _context.Orders.MaxAsync(e => e.OrderNumber) + 1 : 1;
                    var maxInvoiceNumber = _context.Orders.Count() > 0 ? await _context.Orders.MaxAsync(e => e.InvoiceNumber) + 1 : 1;
                    var order = new Order()
                    {
                        AccountNumber = customer.AccountNumber,
                        AmountPaid = 0,
                        Balance = amount,
                        BillAddress = customer.AddressLine1,
                        BillCity = customer.City,
                        BillContactName = customer.ContactName,
                        BillPhoneNumber = customer.PhoneNumber,
                        BillState = customer.State,
                        BillZipCode = customer.ZipCode,
                        BillZone = "",
                        CreatedBy = param.UserName,
                        CreatedDate = currentDate,
                        CustomerId = customer.Id,
                        CustomerName = customer.CustomerName,
                        DeliveryDate = currentDate,
                        DeliveryType = 0,
                        InvoiceNumber = maxInvoiceNumber,
                        IsActive = true,
                        IsDeleted = false,
                        IsHoldInvoice = false,
                        IsPrinted = false,
                        IsQuote = false,
                        OrderDate = currentDate,
                        OrderedBy = param.UserName,
                        OrderNumber = maxOrderNumber,
                        OrderStatusId = 7, // Overpayment
                        OrderStatusName = "Overpayment",
                        //OriginalInvoiceNumber = param.OriginalInvoiceNumber,
                        PaymentTermId = customer.PaymentTermId,
                        PaymentTermName = paymentTerms.Find(e => e.Id == customer.PaymentTermId).TermName,
                        PhoneNumber = customer.PhoneNumber,
                        PriceLevelId = 0,
                        PriceLevelName = "",
                        PurchaseOrderNumber = "Overpayment",
                        ShipAddress = customer.AddressLine1,
                        ShipCity = customer.City,
                        ShipContactName = customer.ContactName,
                        ShipAddressName = customer.CustomerName,
                        ShipPhoneNumber = customer.PhoneNumber,
                        ShipState = customer.State,
                        ShipZipCode = customer.ZipCode,
                        ShipZone = "",
                        TotalAmount = amount,
                        User = param.UserName,
                        WarehouseId = 1,
                        WarehouseName = "",
                        InvoiceNotes = param.Notes,
                    };
                    await _context.Orders.AddAsync(order);
                    await _context.SaveChangesAsync();

                    if (product != null)
                    {
                        var orderDetail = new OrderDetail()
                        {
                            Brand = product.Brand,
                            CategoryId = product.CategoryId.Value,
                            CreatedBy = param.UserName,
                            CreatedDate = currentDate,
                            DiscountedPrice = 0,
                            IsActive = true,
                            IsDeleted = false,
                            ListPrice = 0,
                            Location = "",
                            MainOEMNumber = product.PartNumber,
                            MainPartsLinkNumber = product.PartNumber,
                            OrderId = order.Id,
                            OrderQuantity = 1,
                            PartDescription = product.PartDescription,
                            PartNumber = product.PartNumber,
                            ProductId = product.Id,
                            TotalAmount = amount,
                            VendorCode = "",
                            VendorOnHand = 0,
                            VendorPartNumber = "",
                            VendorPrice = 0
                        };

                        await _context.OrderDetails.AddAsync(orderDetail);
                        await _context.SaveChangesAsync();
                    }

                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            
            return result;
        }

        public async Task<bool> Update(Order order)
        {
            try
            {
                List<OrderDetail> orderDetailsNew = new List<OrderDetail>();
                List<OrderDetail> orderDetailsUpdate = new List<OrderDetail>();
                List<WarehouseTracking> warehouseTrackingList = new List<WarehouseTracking>();
                var userNotificationList = new List<UserNotificationEmailDTO>();

                // Convert Dates back to UTC prior to saving
                order.OrderDate = order.OrderDate.ToUniversalTime();
                order.CreatedDate = order.CreatedDate.ToUniversalTime();

                _context.Orders.Update(order);
                //await _context.SaveEntitiesAsync();

                orderDetailsNew = order.OrderDetails.Where(e => e.Id == 0).ToList();
                orderDetailsUpdate = order.OrderDetails.Where(e => e.Id > 0).ToList();

                if (orderDetailsNew.Count > 0)
                {
                    await _context.OrderDetails.AddRangeAsync(orderDetailsNew);

                    foreach (var orderDetail in orderDetailsNew)
                    {
                        //Add Warehouse Tracking
                        if (!string.IsNullOrWhiteSpace(orderDetail.WarehouseTracking))
                        {
                            var modBy = order.ModifiedBy != null ? order.ModifiedBy : order.CreatedBy;
                            var modDate = order.ModifiedDate != null ? order.ModifiedDate.Value : order.CreatedDate;

                            warehouseTrackingList.Add(
                                new WarehouseTracking()
                                {
                                    CreatedBy = modBy,
                                    CreatedDate = modDate.ToUniversalTime(),
                                    Description = orderDetail.WarehouseTracking,
                                    Id = 0,
                                    IsActive = true,
                                    IsDeleted = false,
                                    OrderDetailId = orderDetail.Id,
                                    OrderId = orderDetail.OrderId,
                                    Status = orderDetail.WarehouseTracking
                                });
                        }
                    }
                }
                
                if (orderDetailsUpdate.Any())
                {
                    // Get if Order Detail was Customer Cancelled or Out of Stock
                    foreach (var od in orderDetailsUpdate)
                    {
                        var original = await _context.OrderDetails.AsNoTracking().FirstOrDefaultAsync(e => e.Id == od.Id);

                        // Get if Order Detail was Customer Cancelled or Out of Stock
                        if (od.StatusId == 1 || od.StatusId == 2)
                        {
                            if (original != null && original.StatusId != od.StatusId)
                            {
                                // User Notification Email
                                var user = await _context.Users.FirstOrDefaultAsync(e => e.UserName == od.CreatedBy);
                                if (user != null)
                                {
                                    userNotificationList.Add(new UserNotificationEmailDTO()
                                    {
                                        OrderNumber = order.OrderNumber != null ? order.OrderNumber.Value : 0,
                                        PartNumber = od.PartNumber,
                                        Subject = od.StatusId == 1 ? "Customer Cancelled" : "Out of Stock",
                                        Username = od.CreatedBy,
                                        Email = user.Email
                                    });
                                }
                            }
                        }

                        if (original != null && original.VendorCode != od.VendorCode)
                        {
                            // User Notification Email
                            var user = await _context.Users.FirstOrDefaultAsync(e => e.UserName == od.CreatedBy);
                            if (user != null)
                            {
                                userNotificationList.Add(new UserNotificationEmailDTO()
                                {
                                    OrderNumber = order.OrderNumber != null ? order.OrderNumber.Value : 0,
                                    PartNumber = od.PartNumber,
                                    Subject = $"Change Vendor from {original.VendorCode} to {od.VendorCode}",
                                    Username = od.CreatedBy,
                                    Email = user.Email
                                });
                            }
                        }
                    }

                    _context.OrderDetails.UpdateRange(orderDetailsUpdate);

                    foreach (var orderDetail in orderDetailsUpdate)
                    {
                        if (!string.IsNullOrWhiteSpace(orderDetail.WarehouseTracking))
                        {
                            ////Get last Warehouse Tracking record of Order Detail
                            //var currentWarehouseTracking = await _context.WarehouseTrackings.Where(e => e.OrderId == orderDetail.OrderId && e.OrderDetailId == orderDetail.Id).OrderByDescending(f => f.Id).FirstOrDefaultAsync();

                            //Get "EXISTING" Warehouse Tracking record of Order Detail
                            var existingWarehouseTracking = await _context.WarehouseTrackings.FirstOrDefaultAsync(e => e.OrderId == orderDetail.OrderId && e.OrderDetailId == orderDetail.Id && 
                                e.Status.Trim().ToLower() == orderDetail.WarehouseTracking.Trim().ToLower());

                            //Add Warehouse Tracking
                            //if ((currentWarehouseTracking == null) || (currentWarehouseTracking.Status.Trim().ToLower() != orderDetail.WarehouseTracking.Trim().ToLower()))
                            if (existingWarehouseTracking == null)
                            {
                                var modBy = order.ModifiedBy != null ? order.ModifiedBy : order.CreatedBy;
                                var modDate = order.ModifiedDate != null ? order.ModifiedDate.Value : order.CreatedDate;

                                warehouseTrackingList.Add(
                                new WarehouseTracking()
                                {
                                    CreatedBy = modBy,
                                    CreatedDate = modDate.ToUniversalTime(),
                                    Description = orderDetail.WarehouseTracking,
                                    Id = 0,
                                    IsActive = true,
                                    IsDeleted = false,
                                    OrderDetailId = orderDetail.Id,
                                    OrderId = orderDetail.OrderId,
                                    Status = orderDetail.WarehouseTracking
                                });
                            }
                        }
                    }


                }

                if (warehouseTrackingList.Count > 0)
                {
                    await _context.WarehouseTrackings.AddRangeAsync(warehouseTrackingList);
                }

                await _context.SaveEntitiesAsync();

                //var vendorCatalogs = new List<VendorCatalog>();
                //foreach (var orderDetail in order.OrderDetails)
                //{
                //    var existing = _context.VendorCatalogs.Any(e => e.PartsLinkNumber == orderDetail.MainPartsLinkNumber &&
                //                                                    e.VendorCode == orderDetail.VendorCode &&
                //                                                    e.VendorPartNumber == orderDetail.VendorPartNumber);
                //    if (!existing)
                //    {
                //        var vendorCatalog = new VendorCatalog()
                //        {
                //            CreatedBy = orderDetail.CreatedBy,
                //            CreatedDate = orderDetail.CreatedDate,
                //            IsActive = orderDetail.IsActive,
                //            IsDeleted = orderDetail.IsDeleted,
                //            OnHand = orderDetail.VendorOnHand,
                //            PartsLinkNumber = orderDetail.MainPartsLinkNumber,
                //            Price = orderDetail.VendorPrice,
                //            VendorCode = orderDetail.VendorCode,
                //            VendorPartNumber = orderDetail.VendorPartNumber
                //        };

                //        vendorCatalogs.Add(vendorCatalog);
                //    }
                //}
                //await _context.VendorCatalogs.AddRangeAsync(vendorCatalogs);

                if (userNotificationList.Any())
                {
                    _emailService.SendUserNotificationEmail(userNotificationList);
                }

                // Update Customer Current Balance
                var customer = await _context.Customers.FirstOrDefaultAsync(e => e.Id == order.CustomerId);
                if (customer != null)
                {
                    var currentBalance = GetStatementReport(DateTime.Now, customer.PaymentTermId != null ? customer.PaymentTermId.Value : 0, new List<int>() { customer.Id }).Result;
                    if (currentBalance != null && currentBalance.Any())
                    {
                        customer.OverBalance = currentBalance[0].TotalDue;
                        _context.Customers.Update(customer);
                        await _context.SaveEntitiesAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
                return false;
            }
        }

        public async Task<bool> ConvertQuoteToOrder(Order order)
        {
            try
            {
                List<OrderDetail> orderDetailsNew = new List<OrderDetail>();
                List<OrderDetail> orderDetailsUpdate = new List<OrderDetail>();
                List<WarehouseTracking> warehouseTrackingList = new List<WarehouseTracking>();

                // Convert Dates back to UTC prior to saving
                order.OrderDate = order.OrderDate.ToUniversalTime();
                order.CreatedDate = order.CreatedDate.ToUniversalTime();

                var orderExist = await _context.Orders.FirstOrDefaultAsync(e => e.OrderNumber == order.OrderNumber);
                if (orderExist != null)
                {
                    var maxOrderNumber = await _context.Orders.MaxAsync(e => e.OrderNumber) + 1;
                    order.OrderNumber = maxOrderNumber;
                    var maxInvoiceNumber = await _context.Orders.MaxAsync(e => e.InvoiceNumber) + 1;
                    order.InvoiceNumber = maxInvoiceNumber;
                }

                _context.Orders.Update(order);
                await _context.SaveEntitiesAsync();

                orderDetailsNew = order.OrderDetails.Where(e => e.Id == 0).ToList();
                orderDetailsUpdate = order.OrderDetails.Where(e => e.Id > 0).ToList();

                if (orderDetailsNew.Count > 0)
                {
                    await _context.OrderDetails.AddRangeAsync(orderDetailsNew);

                    foreach (var orderDetail in orderDetailsNew)
                    {
                        //Add Warehouse Tracking
                        if (!string.IsNullOrWhiteSpace(orderDetail.WarehouseTracking))
                        {
                            var modBy = order.ModifiedBy != null ? order.ModifiedBy : order.CreatedBy;
                            var modDate = order.ModifiedDate != null ? order.ModifiedDate.Value : order.CreatedDate;

                            warehouseTrackingList.Add(
                                new WarehouseTracking()
                                {
                                    CreatedBy = modBy,
                                    CreatedDate = modDate.ToUniversalTime(),
                                    Description = orderDetail.WarehouseTracking,
                                    Id = 0,
                                    IsActive = true,
                                    IsDeleted = false,
                                    OrderDetailId = orderDetail.Id,
                                    OrderId = orderDetail.OrderId,
                                    Status = orderDetail.WarehouseTracking
                                });
                        }
                    }
                }

                if (orderDetailsUpdate.Count > 0)
                {
                    _context.OrderDetails.UpdateRange(orderDetailsUpdate);

                    foreach (var orderDetail in orderDetailsUpdate)
                    {
                        if (!string.IsNullOrWhiteSpace(orderDetail.WarehouseTracking))
                        {
                            ////Get last Warehouse Tracking record of Order Detail
                            //var currentWarehouseTracking = await _context.WarehouseTrackings.Where(e => e.OrderId == orderDetail.OrderId && e.OrderDetailId == orderDetail.Id).OrderByDescending(f => f.Id).FirstOrDefaultAsync();

                            //Get "EXISTING" Warehouse Tracking record of Order Detail
                            var existingWarehouseTracking = await _context.WarehouseTrackings.FirstOrDefaultAsync(e => e.OrderId == orderDetail.OrderId && e.OrderDetailId == orderDetail.Id &&
                                e.Status.Trim().ToLower() == orderDetail.WarehouseTracking.Trim().ToLower());

                            //Add Warehouse Tracking
                            //if ((currentWarehouseTracking == null) || (currentWarehouseTracking.Status.Trim().ToLower() != orderDetail.WarehouseTracking.Trim().ToLower()))
                            if (existingWarehouseTracking == null)
                            {
                                var modBy = order.ModifiedBy != null ? order.ModifiedBy : order.CreatedBy;
                                var modDate = order.ModifiedDate != null ? order.ModifiedDate.Value : order.CreatedDate;

                                warehouseTrackingList.Add(
                                new WarehouseTracking()
                                {
                                    CreatedBy = modBy,
                                    CreatedDate = modDate.ToUniversalTime(),
                                    Description = orderDetail.WarehouseTracking,
                                    Id = 0,
                                    IsActive = true,
                                    IsDeleted = false,
                                    OrderDetailId = orderDetail.Id,
                                    OrderId = orderDetail.OrderId,
                                    Status = orderDetail.WarehouseTracking
                                });
                            }
                        }
                    }


                }

                if (warehouseTrackingList.Count > 0)
                {
                    await _context.WarehouseTrackings.AddRangeAsync(warehouseTrackingList);
                }

                // Turn Off Customer Bypass Credit Limit
                var customer = await _context.Customers.FirstOrDefaultAsync(e => e.Id == order.CustomerId);
                if (customer != null)
                {
                    var currentBalance = GetStatementReport(DateTime.Now, customer.PaymentTermId != null ? customer.PaymentTermId.Value : 0, new List<int>() { customer.Id }).Result;
                    if (currentBalance != null && currentBalance.Any())
                    {
                        customer.OverBalance = currentBalance[0].TotalDue;
                    }

                    customer.IsBypassCreditLimit = false;
                    _context.Customers.Update(customer);
                }

                await _context.SaveEntitiesAsync();
                return true;
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateOrderStatus(Order order)
        {
            try
            {
                List<OrderDetail> orderDetailsNew = new List<OrderDetail>();
                List<OrderDetail> orderDetailsUpdate = new List<OrderDetail>();
                List<WarehouseTracking> warehouseTrackingList = new List<WarehouseTracking>();

                // Convert Dates back to UTC prior to saving
                order.OrderDate = order.OrderDate.ToUniversalTime();
                order.CreatedDate = order.CreatedDate.ToUniversalTime();

                var open = order.OrderDetails.Where(e => e.StatusId != 4).ToList();

                if (!open.Any())
                {
                    order.OrderStatusId = 4;
                    order.OrderStatusName = "Delivered";

                    //// Add Warehouse Tracking
                    //foreach (var orderDetail in order.OrderDetails.Where(e => e.StatusId == 4))
                    //{
                    //    warehouseTrackingList.Add(
                    //        new WarehouseTracking()
                    //        {
                    //            CreatedBy = order.ModifiedBy != null ? order.ModifiedBy : order.CreatedBy,
                    //            CreatedDate = order.ModifiedDate != null ? order.ModifiedDate.Value : order.CreatedDate,
                    //            Description = "Item Delivered",
                    //            IsActive = true,
                    //            IsDeleted = false,
                    //            OrderDetailId = orderDetail.Id,
                    //            OrderId = orderDetail.OrderId,
                    //            Status = $"Item Delivered - {orderDetail.PartNumber}"
                    //        });
                    //}
                }

                _context.Orders.Update(order);
                _context.OrderDetails.UpdateRange(order.OrderDetails);
                await _context.SaveEntitiesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateOrderInspectedCode(Order order)
        {
            try
            {
                List<WarehouseTracking> warehouseTrackingList = new List<WarehouseTracking>();

                // Convert Dates back to UTC prior to saving
                order.OrderDate = order.OrderDate.ToUniversalTime();
                order.CreatedDate = order.CreatedDate.ToUniversalTime();

                _context.Orders.Update(order);
                _context.OrderDetails.UpdateRange(order.OrderDetails);

                var origOrder = await _context.Orders.FirstOrDefaultAsync(e => e.InvoiceNumber == order.OriginalInvoiceNumber);
                if (origOrder != null)
                {
                    origOrder.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == origOrder.Id).ToListAsync();
                }
                
                var inspectedList = order.OrderDetails.Where(e => e.RGAInspectedCode != null && e.RGAInspectedCode > 0 && !e.IsCreditMemoCreated).ToList();

                if (origOrder != null)
                {
                    // Add Warehouse Tracking
                    foreach (var orderDetail in inspectedList)
                    {
                        // RGA
                        warehouseTrackingList.Add(
                            new WarehouseTracking()
                            {
                                CreatedBy = order.ModifiedBy != null ? order.ModifiedBy : order.CreatedBy,
                                CreatedDate = order.ModifiedDate != null ? order.ModifiedDate.Value.ToUniversalTime() : order.CreatedDate.ToUniversalTime(),
                                Description = "Item Inspected",
                                IsActive = true,
                                IsDeleted = false,
                                OrderDetailId = orderDetail.Id,
                                OrderId = orderDetail.OrderId,
                                Status = $"Inspected Code - {GetCodeDescription(orderDetail.RGAInspectedCode)}"
                            });

                        // Original Order
                        var od = origOrder.OrderDetails.Where(e => e.ProductId == orderDetail.ProductId).FirstOrDefault();
                        if (od != null)
                        {
                            warehouseTrackingList.Add(
                            new WarehouseTracking()
                            {
                                CreatedBy = order.ModifiedBy != null ? order.ModifiedBy : order.CreatedBy,
                                CreatedDate = order.ModifiedDate != null ? order.ModifiedDate.Value.ToUniversalTime() : order.CreatedDate.ToUniversalTime(),
                                Description = "Item Inspected",
                                IsActive = true,
                                IsDeleted = false,
                                OrderDetailId = od.Id,
                                OrderId = od.OrderId,
                                Status = $"Inspected Code - {GetCodeDescription(orderDetail.RGAInspectedCode)}"
                            });
                        }

                        //<<<--- Do Background task accordingly --->>>
                        int warehouseId = origOrder.BillState == "NV" ? 2 : 1;
                        string location = !string.IsNullOrEmpty(orderDetail.RGALocation) ? orderDetail.RGALocation.Trim().ToLower() : string.Empty;

                        var warehouseLocation = new WarehouseLocation();

                        // Asked to be removed 03/11/2024
                        ////Add to Location 77 for CA, Location 88 for NV
                        //if (orderDetail.RGAInspectedCode == 3)
                        //{
                        //    location = warehouseId == 2 ? "88" : "77";
                        //    warehouseLocation = await _context.WarehouseLocations.FirstOrDefaultAsync(e => e.Location.Trim() == location);
                        //}
                        //else
                        //{
                        //    warehouseLocation = await _context.WarehouseLocations.FirstOrDefaultAsync(e => e.Location.Trim().ToLower() == location && e.WarehouseId == warehouseId);
                        //}

                        warehouseLocation = await _context.WarehouseLocations.FirstOrDefaultAsync(e => e.Location.Trim().ToLower() == location && e.WarehouseId == warehouseId);

                        if (warehouseLocation != null)
                        {
                            int productId = orderDetail.ProductId;

                            //Required Enter new Partnumber" then Enter Warehouse Location
                            if (orderDetail.RGAInspectedCode == 4)
                            {
                                string partNumber = !string.IsNullOrEmpty(orderDetail.RGAPartNumber) ? orderDetail.RGAPartNumber.Trim().ToLower() : string.Empty;
                                var product = await _context.Products.FirstOrDefaultAsync(e => e.PartNumber.Trim().ToLower() == partNumber);
                                productId = product != null ? product.Id : orderDetail.ProductId;
                            }
                            


                            var ws = await _context.WarehouseStocks.FirstOrDefaultAsync(e => e.WarehouseLocationId == warehouseLocation.Id && e.ProductId == productId);
                            if (ws != null)
                            {
                                ws.Quantity += orderDetail.OrderQuantity;
                                _context.WarehouseStocks.Update(ws);
                            }
                            else //Create Warehouse Stock
                            {
                                var newWS = new WarehouseStock()
                                {
                                    CreatedBy = orderDetail.CreatedBy,
                                    CreatedDate = orderDetail.CreatedDate,
                                    IsActive = orderDetail.IsActive,
                                    IsDeleted = orderDetail.IsDeleted,
                                    ProductId = productId,
                                    Quantity = orderDetail.OrderQuantity,
                                    WarehouseId = warehouseLocation.WarehouseId,
                                    WarehouseLocationId = warehouseLocation.Id,
                                };

                                await _context.WarehouseStocks.AddAsync(newWS);
                            }
                        }

                        //Update Product Current Cost if Return to Stock and Part came from Vendor
                        if (orderDetail.RGAInspectedCode == 1 && !string.IsNullOrEmpty(orderDetail.VendorCode.Trim())) 
                        {
                            var stockQuantity = 0;
                            var product = await _context.Products.FirstOrDefaultAsync(e => e.Id == orderDetail.ProductId);
                            if (product != null)
                            {
                                var stocks = await _context.WarehouseStocks.Where(e => e.ProductId == product.Id).ToListAsync();
                                if (stocks.Any())
                                {
                                    stockQuantity = stocks.Sum(q => q.Quantity);
                                }

                                if (stockQuantity > 0)
                                {
                                    product.CurrentCost = ((product.CurrentCost * stockQuantity) + (orderDetail.VendorPrice * orderDetail.OrderQuantity)) / (stockQuantity + orderDetail.OrderQuantity);
                                }
                                else
                                {
                                    product.CurrentCost = orderDetail.VendorPrice;
                                }

                                _context.Products.Update(product);
                            }
                        }

                        //Update Purchase Order Detail Status to Return Requested if Return To Vendor
                        if (orderDetail.RGAInspectedCode == 3 && !string.IsNullOrEmpty(orderDetail.VendorCode.Trim()))
                        {
                            // Get Purchase Order Detail
                            var purchaseOrderDetail = await _context.PurchaseOrderDetails.Where(e => e.OrderId == origOrder.Id && e.PartNumber.Trim().ToLower() == orderDetail.PartNumber.Trim().ToLower()).OrderBy(o => o.Id).LastOrDefaultAsync();
                            if (purchaseOrderDetail != null)
                            {
                                purchaseOrderDetail.StatusId = 4;
                                purchaseOrderDetail.PODetailStatus = "Return Requested";
                                _context.PurchaseOrderDetails.Update(purchaseOrderDetail);
                            }
                        }
                    }

                    await _context.WarehouseTrackings.AddRangeAsync(warehouseTrackingList);
                }

                await _context.SaveEntitiesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string GetCodeDescription(int? rgaInspectedCode = 0)
        {
            //{ id: 1, code: '' }, //- "Required - Enter Warehouse Location"
            //{ id: 2, code: ''},
            //{ id: 3, code: ''}, //- Add to Location 77 for CA , Location 88 for NV
            //{ id: 4, code: ''}, //- "Required Enter new Partnumber" then Enter Warehouse Location"
            //{ id: 5, code: ''}

            string result = string.Empty;
            switch (rgaInspectedCode)
            {
                case 1:
                    result = "Return to Stock";
                    break;
                case 2:
                    result = "Damaged - Trash";
                    break;
                case 3:
                    result = "Return to Vendor";
                    break;
                case 4:
                    result = "Mislabel";
                    break;
                case 5:
                    result = "Did not return";
                    break;
            }

            return result;
        }

        public async Task<bool> UpdateOrderSummary(Order order)
        {
            try
            {
                // Convert Dates back to UTC prior to saving
                order.OrderDate = order.OrderDate.ToUniversalTime();
                order.CreatedDate = order.CreatedDate.ToUniversalTime();

                _context.Orders.Update(order);
                await _context.SaveEntitiesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateCreditMemo(Order order)
        {
            try
            {
                // Convert Dates back to UTC prior to saving
                order.OrderDate = order.OrderDate.ToUniversalTime();
                order.CreatedDate = order.CreatedDate.ToUniversalTime();
                order.ModifiedDate = order.ModifiedDate.Value.ToUniversalTime();

                _context.Orders.Update(order);
                await _context.SaveEntitiesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<List<Order>> Delete(List<int> orderIds)
        {
            try
            {
                var orders = _context.Orders.Where(a => orderIds.Contains(a.Id)).ToList();
                _context.Orders.RemoveRange(orders);
                await _context.SaveEntitiesAsync();
                return await _context.Orders.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteOrderDetail(int orderDetailId)
        {
            try
            {
                var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(e => e.Id == orderDetailId);
                if (orderDetail != null)
                {
                    orderDetail.IsDeleted = true;
                    _context.OrderDetails.Update(orderDetail);
                    await _context.SaveEntitiesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<Order>> SoftDelete(List<int> orderIds)
        {
            try
            {
                var orders = _context.Orders.Where(a => orderIds.Contains(a.Id)).ToList();
                orders.ForEach(a => { a.IsDeleted = true; });
                _context.Orders.UpdateRange(orders);
                await _context.SaveEntitiesAsync();
                return await _context.Orders.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdatePrintedInvoice(List<int> orderIds)
        {
            try
            { 
                var orderList = await _context.Orders.Where(e => orderIds.Contains(e.Id)).ToListAsync();
                orderList.ForEach(order => { order.IsPrinted = true; });

                _context.Orders.UpdateRange(orderList);
                await _context.SaveEntitiesAsync();
                
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion
    }
}
