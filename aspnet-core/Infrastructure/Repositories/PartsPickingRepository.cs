using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.DTO.Paginated;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;
using Service.Email;

namespace Infrastucture.Repositories
{
    public class PartsPickingRepository : IPartsPickingRepository
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

        public PartsPickingRepository(DataContext context, IEmailService emailService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _emailService = emailService;
        }

        #region Get Data
        public async Task<List<PartsPicking>> GetPartsPickings()
        {
            return await _context.PartsPickings.ToListAsync();
        }

        public async Task<PartsPicking?> GetPartsPicking(int partsPickingId)
        {
            var result = await _context.PartsPickings.FindAsync(partsPickingId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<PaginatedListDTO<PartsPicking>> GetPartsPickingsPaginated(int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search)
        {
            var recordCount = (
                from partsPicking in _context.PartsPickings
                where partsPicking.IsActive == true && partsPicking.IsDeleted == false &&
                    (
                     //string.IsNullOrEmpty(search) ? true : partsPicking.VendorCode.Trim().ToLower().Contains(search) ||
                     //string.IsNullOrEmpty(search) ? true : partsPicking.VendorPO.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : partsPicking.PickNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : partsPicking.PickStatus.Trim().ToLower().Contains(search))
                select partsPicking)
                .Distinct()
                .Count();

            var partsPickings = await (
                from partsPicking in _context.PartsPickings
                where partsPicking.IsActive == true && partsPicking.IsDeleted == false &&
                    (
                     //string.IsNullOrEmpty(search) ? true : partsPicking.VendorCode.Trim().ToLower().Contains(search) ||
                     //string.IsNullOrEmpty(search) ? true : partsPicking.VendorPO.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : partsPicking.PickNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : partsPicking.PickStatus.Trim().ToLower().Contains(search))
                select partsPicking)
                .Distinct()
                .OrderByDescending(e => e.Id)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            foreach ( var partsPicking in partsPickings )
            {
                var partsPickingDetails = await _context.PartsPickingDetails.Where(e => e.PartsPickingId == partsPicking.Id && e.IsActive && !e.IsDeleted).ToListAsync();
                partsPicking.PartsPickingDetails = partsPickingDetails;

                foreach (var partsPickingDetail in partsPicking.PartsPickingDetails)
                {
                    partsPickingDetail.WarehouseLocations = await GetWarehouseParts(partsPickingDetail.ProductId); //await _warehouseRepository.GetWarehousePartsByProductId(partsPickingDetail.ProductId);
                }
            }

            var result = new PaginatedListDTO<PartsPicking>()
            {
                Data = partsPickings,
                RecordCount = recordCount
            };

            return result;
        }

        public async Task<List<StockOrderDetailDTO>> GetStockOrderDetails(int warehouseFilter)
        {
            var result = new List<StockOrderDetailDTO>();
            try
            {
                var orderDetails = await(
                    from order in _context.Orders
                    join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                    join warehouseLocation in _context.WarehouseLocations on orderDetail.WarehouseLocationId equals warehouseLocation.Id
                    where order.IsActive && !order.IsDeleted && !order.IsQuote && (warehouseFilter == 0 ? true : warehouseLocation.WarehouseId == warehouseFilter) &&
                        (order.OrderStatusId != 3 && order.OrderStatusId != 5 && order.OrderStatusId != 6) &&
                        (string.IsNullOrWhiteSpace(orderDetail.WarehouseTracking) || orderDetail.WarehouseTracking.Trim() == "") &&
                         orderDetail.StatusId == null &&
                        (string.IsNullOrWhiteSpace(orderDetail.VendorCode) || orderDetail.VendorCode.Trim() == "") &&  
                         orderDetail.IsActive && !orderDetail.IsDeleted
                    select orderDetail)
                    .OrderBy(e => e.Location)
                    .ToListAsync();

                foreach (var orderDetail in orderDetails)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(e => e.Id == orderDetail.ProductId);
                    var sequence = await _context.Sequences.FirstOrDefaultAsync(e => e.Id == product.SequenceId);
                    var order = await _context.Orders.FirstOrDefaultAsync(e => e.Id == orderDetail.OrderId);
                    List<WarehousePartDTO> warehouseLocations = await GetWarehouseParts(orderDetail.ProductId); //await _warehouseRepository.GetWarehousePartsByProductId(orderDetail.ProductId);

                    if (order != null && warehouseLocations != null && warehouseLocations.Count > 0)
                    {
                        var defaultWarehouseLocation = warehouseLocations.FirstOrDefault(e => e.Location == orderDetail.Location);

                        result.Add(new StockOrderDetailDTO()
                        {
                            OrderId = orderDetail.OrderId,
                            OrderDetailId = orderDetail.Id,
                            ProductId = orderDetail.ProductId,
                            MainPartsLinkNumber = orderDetail.MainPartsLinkNumber,
                            Sequence = sequence != null ? sequence.CategoryGroupDescription : "",
                            CreatedBy = orderDetail.CreatedBy,
                            CreatedDate = orderDetail.CreatedDate.ToLocalTime(),
                            DeliveryMethod = order.DeliveryType == 1 ? "Delivery" : order.DeliveryType == 2 ? "Pickup" : "Shipping",
                            OrderDate = order.OrderDate.ToLocalTime(),
                            Id = orderDetail.Id,
                            IsActive = orderDetail.IsActive,
                            IsDeleted = orderDetail.IsDeleted,
                            ModifiedBy = orderDetail.ModifiedBy,
                            ModifiedDate = orderDetail.ModifiedDate,
                            OrderNumber = order != null ? (order.OrderNumber != null ? order.OrderNumber.Value : 0) : 0,
                            PurchaseOrderNumber = order != null ? (order.PurchaseOrderNumber != null ? order.PurchaseOrderNumber : "") : "",
                            OrderQuantity = orderDetail.OrderQuantity,
                            PartDescription = orderDetail.PartDescription,
                            PartNumber = orderDetail.PartNumber,
                            PODetailStatus = "",
                            ShipZone = order != null ? order.ShipZone : "",
                            StockQuantity = defaultWarehouseLocation != null ? defaultWarehouseLocation.Quantity : warehouseLocations[0].Quantity,
                            StockLocation = orderDetail.Location,
                            WarehouseLocations = warehouseLocations,
                            CustomerName = order != null ? order.ShipAddressName : "",
                            DeliveryDate = order != null ? order.DeliveryDate.ToLocalTime() : DateTime.MinValue,
                            DeliveryRoute = order != null ? order.DeliveryRoute != null ? order.DeliveryRoute : 0 : 0
                        }); ;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        private async Task<List<WarehousePartDTO>> GetWarehouseParts(int productId)
        {
            var warehousePartsList = new List<WarehousePartDTO>();
            var warehouseStocks = await(
                from ws in _context.WarehouseStocks
                where ws.ProductId == productId && ws.Quantity > 0
                select ws)
                .ToListAsync();

            foreach (var w in warehouseStocks)
            {
                var loc = _context.WarehouseLocations.Where(e => e.Id == w.WarehouseLocationId).FirstOrDefaultAsync();

                warehousePartsList.Add(new WarehousePartDTO()
                {
                    WarehouseLocationId = w.WarehouseLocationId,
                    WarehouseStockId = w.Id,
                    ProductId = productId,
                    Quantity = w.Quantity,
                    WarehouseId = w.WarehouseId,
                    Height = loc.Result != null ? loc.Result.Height : 0,
                    Zoning = loc.Result != null ? loc.Result.Zoning : 0,
                    Location = loc.Result != null ? loc.Result.Location : "",
                    CreatedBy = w.CreatedBy,
                    CreatedDate = w.CreatedDate,
                });
            }

            return warehousePartsList;
        }
        #endregion

        #region Save Data
        public async Task<bool> Create(PartsPicking partsPicking)
        {
            try
            {
                var warehouseTrackingList = new List<WarehouseTracking>();
                var orderDetailList = new List<OrderDetail>();

                // Get PickNumber Max + 1
                int maxPickNumber = _context.PartsPickings.Count() > 0 ? _context.PartsPickings.Max(e => Convert.ToInt32(e.PickNumber.Substring(2))) + 1 : 100;
                partsPicking.PickNumber = $"PK{maxPickNumber}";

                await _context.PartsPickings.AddAsync(partsPicking);
                await _context.SaveEntitiesAsync();

                // Save PO Details
                foreach (var poDetail in partsPicking.PartsPickingDetails)
                {
                    poDetail.PartsPickingId = partsPicking.Id;

                    warehouseTrackingList.Add(
                        new WarehouseTracking()
                        {
                            CreatedBy = poDetail.CreatedBy,
                            CreatedDate = poDetail.CreatedDate.ToUniversalTime(),
                            Description = "Picking Stage - " + partsPicking.PickNumber,
                            IsActive = true,
                            IsDeleted = false,
                            OrderDetailId = poDetail.OrderDetailId,
                            OrderId = poDetail.OrderId,
                            Status = "Picking Stage - " + partsPicking.PickNumber,
                        });

                    var orderDetail = await _context.OrderDetails.FindAsync(poDetail.OrderDetailId);
                    if (orderDetail != null)
                    {
                        //if (orderDetail.PickedQuantity != null)
                        //{
                        //    orderDetail.PickedQuantity += pod
                        //}
                        orderDetail.WarehouseTracking = "Picking Stage - " + partsPicking.PickNumber;
                        orderDetailList.Add(orderDetail);
                    }
                }

                await _context.PartsPickingDetails.AddRangeAsync(partsPicking.PartsPickingDetails);
                await _context.WarehouseTrackings.AddRangeAsync(warehouseTrackingList);
                _context.OrderDetails.UpdateRange(orderDetailList);
                await _context.SaveEntitiesAsync();

                return true;
            }
            catch ( Exception ex )
            {
                return false;
            }
        }

        public async Task<bool> Update(PartsPicking partsPicking)
        { 
            try
            {
                var partsPickingDetailList = new List<PartsPickingDetail>();
                var orderDetailList = new List<OrderDetail>();
                var skippedList = new List<UserNotificationEmailDTO>();

                if (partsPicking.PartsPickingDetails.FindIndex(e => e.StatusId == 1) == -1) 
                {
                    // Convert Dates back to UTC prior to saving
                    partsPicking.PartsPickingDate = partsPicking.PartsPickingDate.ToUniversalTime();
                    partsPicking.CreatedDate = partsPicking.CreatedDate.ToUniversalTime();

                    if (partsPicking.PartsPickingDetails.FindIndex(e => e.StatusId == 3) == -1)
                    {
                        partsPicking.StatusId = 2;
                        partsPicking.PickStatus = "Picked at location";
                    }
                    else
                    {
                        partsPicking.StatusId = 3;
                        partsPicking.PickStatus = "Partially Picked";
                    }
                
                    _context.PartsPickings.Update(partsPicking);
                }

                foreach (var partsPickingDetail in partsPicking.PartsPickingDetails)
                {
                    var ppDetail = _context.PartsPickingDetails.FirstOrDefault(e => e.Id == partsPickingDetail.Id);

                    if (ppDetail != null)
                    {
                        if (partsPickingDetail.StatusId != ppDetail.StatusId)
                        {
                            ppDetail.StockLocation = partsPickingDetail.StockLocation;
                            ppDetail.StockQuantity = partsPickingDetail.StockQuantity;
                            ppDetail.PPDetailStatus = partsPickingDetail.PPDetailStatus;
                            ppDetail.StatusId = partsPickingDetail.StatusId;
                            ppDetail.ModifiedBy = partsPicking.ModifiedBy;
                            ppDetail.ModifiedDate = partsPicking.ModifiedDate.Value.ToUniversalTime();

                            // If Status is Picked at location --> Update warehouse Stocks
                            if (partsPickingDetail.StatusId == 2)
                            {
                                var stocks = await (
                                    from ws in _context.WarehouseStocks
                                    join wl in _context.WarehouseLocations on ws.WarehouseLocationId equals wl.Id
                                    where ws.ProductId == partsPickingDetail.ProductId && wl.Location.Trim().ToLower() == partsPickingDetail.StockLocation.Trim().ToLower()
                                    select ws)
                                    .Distinct()
                                    .FirstOrDefaultAsync();
                                
                                if (stocks != null)
                                {
                                    //ppDetail.StockQuantity = stocks.Quantity;
                                    stocks.Quantity -= partsPickingDetail.OrderQuantity;
                                    stocks.ModifiedBy = partsPicking.ModifiedBy;
                                    stocks.ModifiedDate = partsPicking.ModifiedDate.Value.ToUniversalTime();
                                    _context.WarehouseStocks.Update(stocks);
                                }
                            }

                            // If Status is Skipped Item --> Update Order Detail Status to 7
                            if (partsPickingDetail.StatusId == 3)
                            {
                                var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(e => e.Id == ppDetail.OrderDetailId);

                                if (orderDetail != null)
                                {
                                    orderDetail.StatusId = 7;
                                    _context.OrderDetails.Update(orderDetail);

                                    // User Notification Email
                                    var order = await _context.Orders.FirstOrDefaultAsync(e => e.Id == orderDetail.OrderId);
                                    var user = await _context.Users.FirstOrDefaultAsync(e => e.UserName == orderDetail.CreatedBy);

                                    if (order != null && user != null)
                                    {
                                        skippedList.Add(new UserNotificationEmailDTO()
                                        {
                                            OrderNumber = order.OrderNumber != null ? order.OrderNumber.Value : 0,
                                            PartNumber = orderDetail.PartNumber,
                                            Subject = "Skipped",
                                            Username = orderDetail.CreatedBy,
                                            Email = user.Email
                                        });
                                    }
                                }
                            }

                            // Add Warehouse Tracking
                            var warehouseTracking = new WarehouseTracking()
                            {
                                CreatedBy = partsPicking.ModifiedBy,
                                CreatedDate = partsPicking.ModifiedDate.Value.ToUniversalTime(),
                                Description = $"{partsPickingDetail.PPDetailStatus} - {partsPicking.PickNumber}",
                                IsActive = true,
                                IsDeleted = false,
                                Notes = null,
                                OrderDetailId = partsPickingDetail.OrderDetailId,
                                OrderId = partsPickingDetail.OrderId,
                                Status = $"{partsPickingDetail.PPDetailStatus} - {partsPicking.PickNumber}",

                            };

                            _context.WarehouseTrackings.Add(warehouseTracking);

                            partsPickingDetailList.Add(ppDetail);
                        }
                    }
                }

                _context.PartsPickingDetails.UpdateRange(partsPickingDetailList);
                

                //foreach (var poDetail in partsPicking.PartsPickingDetails)
                //{
                //    var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(e => e.Id == poDetail.OrderDetailId);
                //    if (orderDetail != null)
                //    {
                //        orderDetail.VendorPrice = poDetail.VendorPrice;
                //        orderDetailList.Add(orderDetail);
                //    }
                //}

                //_context.OrderDetails.UpdateRange(orderDetailList);

                await _context.SaveEntitiesAsync();
                
                if (skippedList.Any())
                {
                    _emailService.SendUserNotificationEmail(skippedList);
                }

                return true;
            }
            catch ( Exception ex )
            {
                return false;
            }
        }

        public async Task<bool> SoftDeletePartsPickingDetail(PartsPickingDetail partsPickingDetail)
        {
            try
            {
                partsPickingDetail.IsDeleted = true;
                _context.PartsPickingDetails.Update(partsPickingDetail);
                
                var warehouseTracking = await _context.WarehouseTrackings.FirstOrDefaultAsync(e => e.OrderId == partsPickingDetail.OrderId && 
                    e.OrderDetailId == partsPickingDetail.OrderDetailId && e.Status.Contains("Ordered") && e.IsActive && !e.IsDeleted);

                if (warehouseTracking != null)
                {
                    warehouseTracking.IsDeleted = true;
                    _context.WarehouseTrackings.Update(warehouseTracking);
                }

                var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(e => e.Id == partsPickingDetail.OrderDetailId);

                if (orderDetail != null)
                {
                    orderDetail.WarehouseTracking = "";
                    _context.OrderDetails.Update(orderDetail);
                }

                await _context.SaveEntitiesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> Delete(PartsPicking partsPicking)
        {
            try
            {
                partsPicking.IsDeleted = true;
                _context.PartsPickings.Update(partsPicking);

                foreach (var partsPickingDetail in partsPicking.PartsPickingDetails) 
                { 
                    partsPickingDetail.IsDeleted = false; 
                }

                _context.PartsPickingDetails.UpdateRange(partsPicking.PartsPickingDetails);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task<List<PartsPicking>> SoftDelete(List<int> partsPickingIds)
        {
            var partsPickings = _context.PartsPickings.Where(a => partsPickingIds.Contains(a.Id)).ToList();
            partsPickings.ForEach(a => { a.IsDeleted = true; });

            _context.PartsPickings.UpdateRange(partsPickings);
            await _context.SaveEntitiesAsync();
            return await _context.PartsPickings.ToListAsync();
        }

        #endregion
    }
}
