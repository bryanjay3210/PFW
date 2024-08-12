using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DropShip;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.DTO.Reports;
using Domain.DomainModel.Helper;
using Domain.DomainModel.Interface;
using Infrastucture;
using Infrastucture.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace Infrastructure.Repositories
{
    public class DropShipRepository : IDropShipRepository
    {
        private readonly DataContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly string _filePath;
        private readonly string _fileName;
        LogWriter _log;

        //private LogWriter _log = new LogWriter("DropShipRepository Initialized");

        private Dictionary<int, string> _orderStatus = new Dictionary<int, string>()
        {
            { 1, "Open" },
            { 2, "Posted" },
            { 3, "Void" },
            { 4, "Complete" }, //Delivered
            { 5, "Credit Memo" },
            { 6, "Refund" },
            { 7, "Overpayment" },
            { 9, "RGA" }
        };

        private Dictionary<int, string> _orderDetailStatus = new Dictionary<int, string>()
        {
            { 0, "Open" },
            { 1, "Customer Cancelled" },
            { 2, "Out Of Stock" },
            { 3, "Back Ordered" },
            { 4, "Complete" }, //Delivered
            { 5, "Credit Memo" },
            { 6, "Retry" },
            { 7, "Skipped Item" },
            { 8, "Discount" },
            { 9, "RGA" }
        };

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public DropShipRepository(DataContext context, IOrderRepository orderRepository, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _orderRepository = orderRepository;
            _log = new LogWriter(configuration);
            _filePath = configuration.GetSection("AppSettings:BellFlowerFilePath").Value;
            _fileName = configuration.GetSection("AppSettings:BellFlowerFileName").Value;
        }

        public async Task<CheckInventoryResponse> CheckInventory(CheckInventoryRequest request)
        {
            var result = new CheckInventoryResponse();

            var stockSettings = await _context.StockSettings.FirstOrDefaultAsync();
            if (stockSettings == null) return result;

            var customer = await _context.Customers.FirstOrDefaultAsync(e => e.Id == 99663);
            if (customer == null) return result;

            //var stateCode = customer.State == "CA" ? 1 : 2;

            foreach (var part in request.Parts)
            {
                //Search Part if exist
                var product = await _context.Products.FirstOrDefaultAsync(e => e.PartNumber == part.Sku && e.Brand == part.Brand && e.IsAPIAllowed && e.IsActive && !e.IsDeleted);
                if (product != null)
                {
                    //Get Warehouse Stocks with Location Name
                    var stocks = await (
                        from stock in _context.WarehouseStocks
                        join location in _context.WarehouseLocations on stock.WarehouseLocationId equals location.Id
                        where stock.ProductId == product.Id && stock.Quantity > 0 && 
                        (stockSettings.IsDropShipCAProduct && stockSettings.IsDropShipNVProduct ? true : 
                            (stockSettings.IsDropShipCAProduct ? stock.WarehouseId == 1 : (stockSettings.IsDropShipNVProduct ? stock.WarehouseId == 2 : false)))
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
                            ProductId = product.Id,
                            Quantity = stock.Quantity > 0 ? stock.Quantity : 0,
                            WarehouseId = stock.WarehouseId,
                            WarehouseLocationId = stock.WarehouseLocationId,
                            CurrentCost = product.CurrentCost != null ? product.CurrentCost.Value : 0
                        }).ToListAsync();


                    if (stocks.Any())
                    {
                        // Get Stock Quantity
                        var stockQty = stocks.Sum(s => s.Quantity);
                        
                        if (stockQty >= part.Quantity)
                        {
                            decimal cost = product.CurrentCost != null ? product.CurrentCost.Value : 0;

                            var newPart = new PartInquiry()
                            {
                                Brand = part.Brand,
                                IsSpecial = part.IsSpecial,
                                OrderItemId = part.OrderItemId,
                                Quantity = part.Quantity,
                                Remarks = "Part is available.",
                                ShipCharge = 0,
                                Sku = part.Sku,
                                Stock = stockQty,
                                VendorCorePrice = 0,
                                VendorPrice = cost + (cost / 10)
                            };

                            result.Parts.Add(newPart);
                        }
                        else
                        {
                            var newPart = new PartInquiry()
                            {
                                Brand = part.Brand,
                                IsSpecial = part.IsSpecial,
                                OrderItemId = part.OrderItemId,
                                Quantity = part.Quantity,
                                Remarks = "Part is out of stock.",
                                ShipCharge = 0,
                                Sku = part.Sku,
                                Stock = 0,
                                VendorCorePrice = 0,
                                VendorPrice = 0
                            };

                            result.Parts.Add(newPart);
                        }
                    }
                    else
                    {
                        var newPart = new PartInquiry()
                        {
                            Brand = part.Brand,
                            IsSpecial = part.IsSpecial,
                            OrderItemId = part.OrderItemId,
                            Quantity = part.Quantity,
                            Remarks = "Part is out of stock.",
                            ShipCharge = 0,
                            Sku = part.Sku,
                            Stock = 0,
                            VendorCorePrice = 0,
                            VendorPrice = 0
                        };

                        result.Parts.Add(newPart);
                    }
                }
                else
                {
                    var newPart = new PartInquiry()
                    {
                        Brand = part.Brand,
                        IsSpecial = part.IsSpecial,
                        OrderItemId = part.OrderItemId,
                        Quantity = part.Quantity,
                        Remarks = "Item not found.",
                        ShipCharge = 0,
                        Sku = part.Sku,
                        Stock = 0,
                        VendorCorePrice = 0,
                        VendorPrice = 0
                    };

                    result.Parts.Add(newPart);
                }

            }
            
            result.TransactionDate = DateTime.Now;
            return result;
        }

        public async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request)
        {
            PlaceOrderResponse result = new PlaceOrderResponse();

            var stockSettings = await _context.StockSettings.FirstOrDefaultAsync();
            if (stockSettings == null) return result;

            try
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(e => e.Id == 99663);
                if (customer == null) return result;

                if (customer != null)
                {
                    var transactionDate = DateTime.Now;
                    var priceLevels = await _context.PriceLevels.ToListAsync();
                    var priceLevel = priceLevels.FirstOrDefault(e => e.Id == customer.PriceLevelId);
                    var priceLevelName = string.Empty;
                    
                    if (priceLevel != null)
                    {
                        priceLevelName = priceLevel.LevelName;
                    }

                    var paymentTerms = await _context.PaymentTerms.ToListAsync();
                    var paymentTerm = paymentTerms.FirstOrDefault(e => e.Id == customer.PaymentTermId);
                    var paymentTermName = string.Empty;
                    
                    if (paymentTerm != null)
                    {
                        paymentTermName = paymentTerm.TermName;
                    }

                    var userName = "DropShip";
                    var address = string.Empty;
                    var city = string.Empty;
                    var state = string.Empty;
                    var zipCode = string.Empty;
                    var zoneName = string.Empty;
                    var contactName = string.Empty;
                    var phone = string.Empty;
                    var email = string.Empty;

                    var billLocation = await _context.Locations.FirstOrDefaultAsync(e => e.CustomerId == customer.Id && e.LocationTypeId == 3);
                    if (billLocation != null)
                    {
                        var contact = await _context.Contacts.Where(e => e.LocationId == billLocation.Id && e.IsDeleted == false).FirstOrDefaultAsync();
                        if (contact != null)
                        {
                            contactName = contact.ContactName;
                            phone = contact.PhoneNumber;
                            email = contact.Email;
                        }

                        var zone = await _context.Zones.Where(e => e.ZipCode.Trim().ToLower() == billLocation.ZipCode.Trim().ToLower()).FirstOrDefaultAsync();
                        if (zone != null)
                        {
                            zoneName = zone.BinCode;
                        }

                        address = $"{billLocation.AddressLine1} {billLocation.AddressLine2}";
                        city = $"{customer.City}";
                        state = $"{customer.State}";
                        zipCode = $"{customer.ZipCode}";
                    }

                    foreach (var orderInfo in request.OrderInfos)
                    {
                        var shipZoneName = string.Empty;
                        var shipZone = await _context.Zones.Where(e => e.ZipCode.Trim().ToLower() == orderInfo.Order.DeliveryPostCode.Trim().ToLower()).FirstOrDefaultAsync();
                        if (shipZone != null)
                        {
                            shipZoneName = shipZone.BinCode;
                        }

                        // Initialize New Order
                        var maxOrderNumber = _context.Orders.Count() > 0 ? _context.Orders.Max(e => e.OrderNumber) + 1 : 1;
                        var maxInvoiceNumber = _context.Orders.Count() > 0 ? _context.Orders.Max(e => e.InvoiceNumber) + 1 : 1;

                        var order = new Order()
                        {
                            Quantity = 0,
                            AccountNumber = customer.AccountNumber,
                            CustomerId = customer.Id,
                            CustomerName = customer.CustomerName,
                            PhoneNumber = customer.PhoneNumber,
                            PriceLevelId = customer.PriceLevelId != null ? customer.PriceLevelId.Value : 0,
                            PaymentTermId = customer.PaymentTermId,
                            OrderStatusId = 1,
                            WarehouseId = 1,
                            PriceLevelName = priceLevelName,
                            PaymentTermName = paymentTermName,
                            OrderStatusName = "Open",
                            WarehouseName = "CA Warehouse",
                            User = userName,
                            BillAddress = address,
                            BillCity = city,
                            BillState = state,
                            BillZipCode = zipCode,
                            BillZone = zoneName,
                            BillContactName = contactName,
                            BillPhoneNumber = phone,
                            ShipAddressName = orderInfo.Order.DeliveryName, //customer.CustomerName,
                            ShipAddress = $"{orderInfo.Order.DeliveryStreet} {orderInfo.Order.DeliverySuburb}",
                            ShipCity = orderInfo.Order.DeliveryCity,
                            ShipState = orderInfo.Order.DeliveryState,
                            ShipZipCode = orderInfo.Order.DeliveryPostCode,
                            ShipZone = shipZoneName,
                            ShipContactName = orderInfo.Order.DeliveryName, //orderInfo.Order.CustomerEmail,
                            ShipPhoneNumber = orderInfo.Order.DeliveryTelephone != null ? orderInfo.Order.DeliveryTelephone : "",
                            Discount = customer.Discount,
                            TaxRate = customer.TaxRate,
                            AmountPaid = 0,
                            OrderedBy = userName,
                            OrderedByEmail = email,
                            OrderedByPhoneNumber = phone,
                            DeliveryType = 3, // Shipping
                            DeliveryDate = transactionDate,
                            DeliveryRoute = 1, // AM
                            OrderDate = transactionDate,
                            IsActive = true,
                            IsDeleted = false,
                            OrderNumber = maxOrderNumber,
                            InvoiceNumber = maxInvoiceNumber,
                            CreatedBy = userName,
                            CreatedDate = transactionDate,
                            OrderDetails = new List<OrderDetail>(),
                            TotalTax = 0,
                            SubTotalAmount = 0,
                            TotalAmount = 0,
                            Balance = 0,
                            PurchaseOrderNumber = orderInfo.Order.VendedId
                        };

                        foreach (var part in orderInfo.Parts)
                        {
                            //Search Part if exist
                            var product = await _context.Products.FirstOrDefaultAsync(e => e.PartNumber == part.Sku && e.Brand == part.Brand && e.IsAPIAllowed && e.IsActive && !e.IsDeleted);

                            if (product != null)
                            {
                                //Get Warehouse Stocks with Location Name
                                var stocks = await (
                                    from stock in _context.WarehouseStocks
                                    join location in _context.WarehouseLocations on stock.WarehouseLocationId equals location.Id
                                    where stock.ProductId == product.Id && stock.Quantity > 0 &&
                                        (stockSettings.IsDropShipCAProduct && stockSettings.IsDropShipNVProduct ? true :
                                        (stockSettings.IsDropShipCAProduct ? stock.WarehouseId == 1 : (stockSettings.IsDropShipNVProduct ? stock.WarehouseId == 2 : false)))
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
                                        ProductId = product.Id,
                                        Quantity = stock.Quantity > 0 ? stock.Quantity : 0,
                                        WarehouseId = stock.WarehouseId,
                                        WarehouseLocationId = stock.WarehouseLocationId,
                                        CurrentCost = product.CurrentCost != null ? product.CurrentCost.Value : 0
                                    }).ToListAsync();

                                if (stocks.Any())
                                {
                                    // Get Stock Quantity
                                    var stockQty = stocks.Sum(s => s.Quantity);

                                    if (stockQty >= part.Quantity)
                                    {
                                        //decimal cost = product.CurrentCost != null ? product.CurrentCost.Value : 0;

                                        var mainPartsLink = string.Empty;
                                        var mainOEM = string.Empty;

                                        var partsList = await _context.ItemMasterlistReferences.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber).ToListAsync();
                                        if (partsList != null && partsList.Any())
                                        {
                                            var partsLink = partsList.Where(e => e.IsMainPartsLink).FirstOrDefault();
                                            mainPartsLink = partsLink != null ? partsLink.PartsLinkNumber : string.Empty;

                                            partsLink = partsList.Where(e => e.IsMainOEM).FirstOrDefault();
                                            mainOEM = partsLink != null ? partsLink.OEMNumber : string.Empty;
                                        }

                                        var vendorList = new List<VendorCatalog>();

                                        if (!string.IsNullOrEmpty(mainPartsLink))
                                        {
                                            vendorList = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim().ToLower() == mainPartsLink.Trim().ToLower()).ToListAsync();
                                        }

                                        var catalogList = await _context.PartsCatalogs.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber && e.ProductId == product.Id).ToListAsync();

                                        decimal cost = product.CurrentCost != null ? product.CurrentCost.Value + (product.CurrentCost.Value / 10) : 0;

                                        decimal wholesalePrice = cost; //GetWholesalePrice(stkPart, customer.PriceLevelId);
                                        decimal discountedPrice = cost;

                                        if (customer.Discount != null)
                                        {
                                            discountedPrice = cost - (cost / 100 * customer.Discount.Value);
                                        }

                                        decimal totalAmount = discountedPrice * part.Quantity;


                                        int currentState = customer.State == "CA" ? 1 : 2;
                                        WarehouseStock currentStock = new WarehouseStock();
                                        string loc = string.Empty;
                                        int warehouseLocationId = 0;
                                        if (stocks != null)
                                        {
                                            currentStock = stocks.Find(e => e.WarehouseId == currentState);
                                            loc = currentStock != null ? currentStock.Location : stocks[0].Location;
                                            warehouseLocationId = currentStock != null ? currentStock.WarehouseLocationId : stocks[0].WarehouseLocationId;
                                        }

                                        var orderDetail = new OrderDetail()
                                        {
                                            OrderId = 0,
                                            ProductId = product.Id,
                                            OrderQuantity = part.Quantity,
                                            Location = loc,
                                            WarehouseLocationId = warehouseLocationId,
                                            PartNumber = product.PartNumber,
                                            PartDescription = product.PartDescription != null ? product.PartDescription : "",
                                            Brand = product.Brand != null ? product.Brand : "",
                                            MainPartsLinkNumber = mainPartsLink != null ? mainPartsLink : "",
                                            MainOEMNumber = mainOEM != null ? mainOEM : "",

                                            VendorCode = "",
                                            VendorPartNumber = "",
                                            VendorPrice = 0,
                                            VendorOnHand = 0,
                                            OnHandQuantity = stockQty,
                                            YearFrom = (catalogList != null && catalogList.Any()) ? catalogList.Min(e => e.YearFrom) : 1900,
                                            YearTo = (catalogList != null && catalogList.Any()) ? catalogList.Max(e => e.YearTo) : 1900,

                                            ListPrice = product.PriceLevel1 != null ? product.PriceLevel1.Value : 0,
                                            WholesalePrice = wholesalePrice,
                                            DiscountedPrice = discountedPrice,
                                            TotalAmount = totalAmount,

                                            PartSize = product.PartSizeId,
                                            CategoryId = product.CategoryId != null ? product.CategoryId.Value : 0,
                                            PartsLinks = partsList != null ? string.Join(",", partsList.Select(e => e.PartsLinkNumber)) : "",
                                            OEMs = partsList != null ? string.Join(",", partsList.Select(e => e.OEMNumber)) : "",
                                            VendorCodes = (vendorList != null && vendorList.Any()) ? string.Join(',', vendorList.Select(e => e.VendorCode)) : "",

                                            IsActive = true,
                                            IsDeleted = false,
                                            CreatedBy = userName,
                                            CreatedDate = transactionDate,

                                            Price = cost,
                                            UnitCost = product.CurrentCost,
                                            //WarehouseTracking
                                        };

                                        order.OrderDetails.Add(orderDetail);

                                        order.Quantity += part.Quantity;
                                        order.TotalTax = 0;
                                        order.SubTotalAmount += totalAmount;
                                        order.TotalAmount += totalAmount;
                                        order.Balance += totalAmount;

                                        part.Status = "Part was included in the Order.";
                                        part.Stock = stockQty;
                                        part.VendorPrice = cost;
                                    }
                                    else
                                    {
                                        part.Status = "Part is out of stock.";
                                    }
                                }
                                else
                                {
                                    part.Status = "Part is out of stock.";
                                }
                            }
                            else
                            {
                                part.Status = "Item not found.";
                            }
                        }

                        var newOrder = await _orderRepository.Create(order);

                        if (newOrder != null && newOrder.Id > 0)
                        {
                            orderInfo.Order.AnError = "Order Been Placed to PerfectFITWest";
                            orderInfo.Order.SupplierOrderId = newOrder.OrderNumber;
                        }
                        else
                        {
                            orderInfo.Order.AnError = "An error was encountered while placing the Order to PerfectFITWest";
                        }
                    }
                }

                result.OrderInfos = request.OrderInfos;
                result.TransactionDate = DateTime.Now;
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<OrderStatusResponse> OrderStatus(OrderStatusRequest request)
        {
            var result = new OrderStatusResponse();
            var statusPart = new StatusPart();

            foreach(var requestOrder in request.Orders)
            {
                var resultOrder = new StatusOrderParts()
                {
                    PONumber = requestOrder.PONumber,
                    SupplierId = requestOrder.SupplierId,
                    Parts = new List<StatusPart>()
                };
                
                var order = await _context.Orders.FirstOrDefaultAsync(e => e.PurchaseOrderNumber == requestOrder.PONumber && e.OrderNumber == requestOrder.SupplierId);
                
                if (order != null)
                {
                    var status = _orderStatus[order.OrderStatusId];
                    
                    var orderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.Id).ToListAsync();

                    if (orderDetails.Any())
                    {
                        foreach (var detail in orderDetails)
                        {
                            var detailStatus = detail.StatusId == null ? _orderDetailStatus[0] : _orderDetailStatus[detail.StatusId.Value];

                            if (order.OrderStatusId == 3)
                            {
                                detailStatus = "Void";
                            }
                            else
                            {
                                if ((detail.ShippedQuantity != null && detail.ShippedQuantity == 0) && detail.OrderQuantity == 0)
                                {
                                    detailStatus = "Void";
                                }
                                else
                                {
                                    if (order.OrderStatusId == 4 && (detail.ShippedQuantity != null && detail.ShippedQuantity == detail.OrderQuantity))
                                    {
                                        detailStatus = status;
                                    }
                                }
                            }

                            var newPart = new StatusPart()
                            { 
                                Carrier = detail.Carrier,
                                QuantityOrdered = detail.OrderQuantity,
                                QuantityShipped = detail.ShippedQuantity != null ? detail.ShippedQuantity.Value : 0,
                                ShipDate = order.ModifiedDate != null ? order.ModifiedDate.Value.ToLocalTime() : null,
                                Sku = detail.PartNumber,
                                Status = detailStatus,
                                Tracking = detail.TrackingNumber
                            };
                            
                            resultOrder.Parts.Add(newPart);
                        }

                        resultOrder.Status = status;
                    }
                    else
                    {
                        resultOrder.Status = "Order Details not found.";
                    }
                }
                else
                {
                    resultOrder.Status = "Order not found.";
                }

                result.Orders.Add(resultOrder);
            }

            result.TransactionDate = DateTime.Now;
            return result;
        }

        private decimal? GetDiscountedPrice(Product product, decimal? discount, int? priceLevelId)
        {
            decimal? originalPrice = 0;
            switch (priceLevelId)
            {
                case 1:
                    {
                        originalPrice = product.PriceLevel1;
                        break;
                    }
                case 2:
                    {
                        originalPrice = product.PriceLevel2;
                        break;
                    }
                case 3:
                    {
                        originalPrice = product.PriceLevel3;
                        break;
                    }
                case 4:
                    {
                        originalPrice = product.PriceLevel4;
                        break;
                    }
                case 5:
                    {
                        originalPrice = product.PriceLevel5;
                        break;
                    }
                case 6:
                    {
                        originalPrice = product.PriceLevel6;
                        break;
                    }
                case 7:
                    {
                        originalPrice = product.PriceLevel7;
                        break;
                    }
                case 8:
                    {
                        originalPrice = product.PriceLevel8;
                        break;
                    }
            }

            if (discount != null)
            {
                return originalPrice - (originalPrice / 100 * discount);
            }

            return originalPrice;
        }
        private decimal? GetWholesalePrice(Product product, int? priceLevelId)
        {
            switch (priceLevelId)
            {
                case 1: return product.PriceLevel1;
                case 2: return product.PriceLevel2;
                case 3: return product.PriceLevel3;
                case 4: return product.PriceLevel4;
                case 5: return product.PriceLevel5;
                case 6: return product.PriceLevel6;
                case 7: return product.PriceLevel7;
                default: return product.PriceLevel8;
            }
        }

        //public async Task<List<BellFlowerDailyReportDTO>> GenerateBellFlowerDailyReport(int triggerHour, string filePath)
        public async void GenerateBellFlowerDailyReport(int triggerHour, int triggerMinute)
        {
            var result = new List<BellFlowerDailyReportDTO>();
            var dt = DateTime.Now;
            
            if (dt.Hour == triggerHour && dt.Minute == triggerMinute && dt.Second == 01)
            {
                _log.LogWrite("Generating Daily Report");
                try
                {
                    var stockSettings = await _context.StockSettings.FirstOrDefaultAsync();
                    if (stockSettings == null)
                    {
                        _log.LogWrite("Stock Settings failed to initialize.");
                        throw new Exception("Stock Settings failed to initialize.");
                    }

                    var customer = await _context.Customers.FirstOrDefaultAsync(e => e.Id == 99663);

                    if (customer == null)
                    {
                        _log.LogWrite("DropShip Customer failed to initialize.");
                        throw new Exception("DropShip Customer failed to initialize.");
                    }

                    var products = await _context.Products.Where(e => e.IsActive && !e.IsDeleted).ToListAsync();
                    _log.LogWrite($"Product Count: {products.Count}");

                    if (!products.Any())
                    {
                        _log.LogWrite("Products failed to initialize.");
                        throw new Exception("Products failed to initialize.");
                    }

                    foreach (var product in products)
                    {
                        //_log.LogWrite($"Processing Product {product.PartNumber} - {product.PartDescription}");
                        var stockQty = 0;
                        var mainPartsLink = string.Empty;
                        var mainOEM = string.Empty;

                        var partsList = await _context.ItemMasterlistReferences.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber).ToListAsync();
                        if (partsList != null && partsList.Any())
                        {
                            var partsLink = partsList.Where(e => e.IsMainPartsLink).FirstOrDefault();
                            mainPartsLink = partsLink != null ? partsLink.PartsLinkNumber : string.Empty;

                            partsLink = partsList.Where(e => e.IsMainOEM).FirstOrDefault();
                            mainOEM = partsLink != null ? partsLink.OEMNumber : string.Empty;
                        }

                        //If IsAPIAllowed then Get Warehouse Stocks with Location Name
                        var stocks = new List<WarehouseStock>();

                        if (product.IsAPIAllowed)
                        {
                            stocks = await (
                            from stock in _context.WarehouseStocks
                            join location in _context.WarehouseLocations on stock.WarehouseLocationId equals location.Id
                            where stock.ProductId == product.Id && stock.Quantity > 0 &&
                                (stockSettings.IsDropShipCAProduct && stockSettings.IsDropShipNVProduct ? true :
                                (stockSettings.IsDropShipCAProduct ? stock.WarehouseId == 1 : (stockSettings.IsDropShipNVProduct ? stock.WarehouseId == 2 : false)))
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
                                ProductId = product.Id,
                                Quantity = stock.Quantity > 0 ? stock.Quantity : 0,
                                WarehouseId = stock.WarehouseId,
                                WarehouseLocationId = stock.WarehouseLocationId,
                                CurrentCost = product.CurrentCost != null ? product.CurrentCost.Value : 0
                            }).ToListAsync();

                            //_log.LogWrite($"Added Product Stock {product.PartNumber} - {product.PartDescription} --> Stock Location Count: {stocks.Count}");
                        }

                        if (stocks.Any())
                        {
                            stockQty = stocks.Sum(s => s.Quantity);
                        }

                        decimal cost = product.CurrentCost != null ? product.CurrentCost.Value : 0;

                        var part = new BellFlowerDailyReportDTO()
                        {
                            description = product.PartDescription != null ? product.PartDescription.Replace(",", " ") : "",
                            old_vendor_sku = mainPartsLink != null ? mainPartsLink : "",
                            stock = stockQty,
                            vendor_brand = product.Brand != null ? product.Brand : "",
                            vendor_brand_code = "",
                            vendor_core_cost = 0,
                            vendor_cost = cost + (cost / 10),
                            vendor_sku = product.PartNumber,
                            vendor_sku_code = ""
                        };

                        result.Add(part);
                        _log.LogWrite($"Added Product {product.PartNumber} - {product.PartDescription} --> Stock Quantity: {stockQty}");
                    }
                }
                catch (Exception ex)
                {
                    _log.LogWrite(ex.Message);
                    throw new Exception(ex.Message, ex);
                }

                _log.LogWrite($"Total Added Products: {result.Count}");

                if (result.Any())
                {
                    _log.LogWrite($"Writing {result.Count} records to CSV");
                    string path = $"{_filePath}{string.Format(_fileName, DateTime.Now.ToString("yyyyMMdd"))}.csv";
                    SaveToCsv(result, path, _log);
                }
            }
        }

        private void SaveToCsv<T>(List<T> reportData, string path, LogWriter log)
        {
            log.LogWrite($"SaveToCsv {path}");
            var lines = new List<string>();
            IEnumerable<PropertyDescriptor> props = TypeDescriptor.GetProperties(typeof(T)).OfType<PropertyDescriptor>();
            var header = string.Join(",", props.ToList().Select(x => x.Name));
            lines.Add(header);
            var valueLines = reportData.Select(row => string.Join(",", header.Split(',').Select(a => row.GetType().GetProperty(a).GetValue(row, null))));
            lines.AddRange(valueLines);
            File.WriteAllLines(path, lines.ToArray());
        }
    }
}
