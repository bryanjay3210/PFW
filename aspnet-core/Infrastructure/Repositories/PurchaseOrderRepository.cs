using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.DTO.Paginated;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Numerics;

namespace Infrastucture.Repositories
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public PurchaseOrderRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<PurchaseOrder>> GetPurchaseOrders()
        {
            return await _context.PurchaseOrders.ToListAsync();
        }

        public async Task<PurchaseOrder?> GetPurchaseOrder(int purchaseOrderId)
        {
            var result = await _context.PurchaseOrders.FindAsync(purchaseOrderId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<PaginatedListDTO<PurchaseOrder>> GetPurchaseOrdersPaginated(int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search)
        {
            var recordCount = (
                from purchaseOrder in _context.PurchaseOrders
                join purchaseOrderDetail in _context.PurchaseOrderDetails on purchaseOrder.Id equals purchaseOrderDetail.PurchaseOrderId
                join vendor in _context.Vendors on purchaseOrder.VendorId equals vendor.Id
                where purchaseOrder.IsActive == true && purchaseOrder.IsDeleted == false &&
                    (string.IsNullOrEmpty(search) ? true : purchaseOrderDetail.OrderNumber.ToString().Contains(search) || 
                     string.IsNullOrEmpty(search) ? true : purchaseOrderDetail.PartNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : purchaseOrderDetail.PODetailStatus.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : purchaseOrderDetail.MainPartsLinkNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : purchaseOrderDetail.VendorPartNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : purchaseOrder.VendorName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : purchaseOrder.VendorCode.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : purchaseOrder.PFWBNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : purchaseOrder.POStatus.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : vendor.PhoneNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : (purchaseOrder.VendorPO == null ? false : purchaseOrder.VendorPO.Trim().ToLower().Contains(search))) 
                select purchaseOrder)
                .Distinct()
                .Count();

            var purchaseOrders = await (
                from purchaseOrder in _context.PurchaseOrders
                join purchaseOrderDetail in _context.PurchaseOrderDetails on purchaseOrder.Id equals purchaseOrderDetail.PurchaseOrderId
                join vendor in _context.Vendors on purchaseOrder.VendorId equals vendor.Id
                where purchaseOrder.IsActive == true && purchaseOrder.IsDeleted == false &&
                    (string.IsNullOrEmpty(search) ? true : purchaseOrderDetail.OrderNumber.ToString().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : purchaseOrderDetail.PartNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : purchaseOrderDetail.PODetailStatus.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : purchaseOrderDetail.MainPartsLinkNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : purchaseOrderDetail.VendorPartNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : purchaseOrder.VendorName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : purchaseOrder.VendorCode.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : purchaseOrder.PFWBNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : purchaseOrder.POStatus.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : vendor.PhoneNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : (purchaseOrder.VendorPO == null ? false : purchaseOrder.VendorPO.Trim().ToLower().Contains(search)))
                select purchaseOrder)
                .Distinct()
                .OrderByDescending(e => e.PFWBNumber)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            foreach ( var purchaseOrder in purchaseOrders )
            {
                var purchaseOrderDetails = await _context.PurchaseOrderDetails.Where(e => e.PurchaseOrderId == purchaseOrder.Id && e.IsActive && !e.IsDeleted).ToListAsync();
                purchaseOrder.PurchaseOrderDetails = purchaseOrderDetails;

                purchaseOrder.TotalAmount = purchaseOrderDetails.Sum(e => e.VendorPrice * e.OrderQuantity);
                purchaseOrder.TotalQuantity = purchaseOrderDetails.Sum(e => e.OrderQuantity);
            }

            var result = new PaginatedListDTO<PurchaseOrder>()
            {
                Data = purchaseOrders,
                RecordCount = recordCount
            };

            return result;
        }

        public async Task<PaginatedListDTO<PurchaseOrder>> GetPurchaseOrdersByDatePaginated(int pageSize, int pageIndex, DateTime fromDate, DateTime toDate)
        {
            DateTime fDate = fromDate;
            DateTime tDate = toDate.AddDays(1);

            var recordCount = (
                from purchaseOrder in _context.PurchaseOrders
                join purchaseOrderDetail in _context.PurchaseOrderDetails on purchaseOrder.Id equals purchaseOrderDetail.PurchaseOrderId
                join vendor in _context.Vendors on purchaseOrder.VendorId equals vendor.Id
                where purchaseOrder.IsActive && !purchaseOrder.IsDeleted && (purchaseOrder.PurchaseOrderDate >= fDate && purchaseOrder.PurchaseOrderDate < tDate)
                select purchaseOrder)
                .Distinct()
                .Count();

            var purchaseOrders = await (
                from purchaseOrder in _context.PurchaseOrders
                join purchaseOrderDetail in _context.PurchaseOrderDetails on purchaseOrder.Id equals purchaseOrderDetail.PurchaseOrderId
                join vendor in _context.Vendors on purchaseOrder.VendorId equals vendor.Id
                where purchaseOrder.IsActive && !purchaseOrder.IsDeleted && (purchaseOrder.PurchaseOrderDate >= fDate && purchaseOrder.PurchaseOrderDate < tDate)
                select purchaseOrder)
                .Distinct()
                .OrderByDescending(e => e.PFWBNumber)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            foreach (var purchaseOrder in purchaseOrders)
            {
                var purchaseOrderDetails = await _context.PurchaseOrderDetails.Where(e => e.PurchaseOrderId == purchaseOrder.Id && e.IsActive && !e.IsDeleted).ToListAsync();
                purchaseOrder.PurchaseOrderDetails = purchaseOrderDetails;

                purchaseOrder.TotalAmount = purchaseOrderDetails.Sum(e => e.VendorPrice * e.OrderQuantity);
                purchaseOrder.TotalQuantity = purchaseOrderDetails.Sum(e => e.OrderQuantity);
            }

            var result = new PaginatedListDTO<PurchaseOrder>()
            {
                Data = purchaseOrders,
                RecordCount = recordCount
            };

            return result;
        }


        public async Task<DailyVendorSalesSummaryDTO> GetDailyVendorSalesSummary(DateTime currentDate)
        {
            var result = new DailyVendorSalesSummaryDTO();

            try
            {
                DateTime frDate = currentDate;
                DateTime toDate = currentDate.AddDays(1);

                var purchaseOrders = await _context.PurchaseOrders.Where(e => e.IsActive && !e.IsDeleted && (e.PurchaseOrderDate >= frDate &&  e.PurchaseOrderDate < toDate)).ToListAsync();

                foreach (var purchaseOrder in purchaseOrders)
                {
                    var purchaseOrderDetails = await _context.PurchaseOrderDetails.Where(e => e.PurchaseOrderId == purchaseOrder.Id && e.IsActive && !e.IsDeleted).ToListAsync();

                    var vendor = result.VendorSummary.Find(e => e.VendorCode == purchaseOrder.VendorCode);
                    if (vendor == null)
                    {
                        result.VendorSummary.Add(new TotalVendorSalesDTO
                        {
                            VendorName = purchaseOrder.VendorName,
                            VendorCode = purchaseOrder.VendorCode,
                            SalesAmount = purchaseOrderDetails.Sum(e => e.VendorPrice),
                            Quantity = purchaseOrderDetails.Sum(e => e.OrderQuantity)
                    });
                    }
                    else
                    {
                        vendor.SalesAmount += purchaseOrderDetails.Sum(e => e.VendorPrice);
                        vendor.Quantity += purchaseOrderDetails.Sum(e => e.OrderQuantity);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        public async Task<DailyVendorSalesSummaryDTO> GetDailyVendorSalesSummaryByDate(DateTime fromDate, DateTime toDate)
        {
            var result = new DailyVendorSalesSummaryDTO();

            try
            {
                DateTime fDate = fromDate;
                DateTime tDate = toDate.AddDays(1);

                var purchaseOrders = await _context.PurchaseOrders.Where(e => e.IsActive && !e.IsDeleted && (e.PurchaseOrderDate >= fDate && e.PurchaseOrderDate < tDate)).ToListAsync();

                foreach (var purchaseOrder in purchaseOrders)
                {
                    var purchaseOrderDetails = await _context.PurchaseOrderDetails.Where(e => e.PurchaseOrderId == purchaseOrder.Id && e.IsActive && !e.IsDeleted).ToListAsync();

                    var vendor = result.VendorSummary.Find(e => e.VendorCode == purchaseOrder.VendorCode);
                    if (vendor == null)
                    {
                        result.VendorSummary.Add(new TotalVendorSalesDTO
                        {
                            VendorName = purchaseOrder.VendorName,
                            VendorCode = purchaseOrder.VendorCode,
                            SalesAmount = purchaseOrderDetails.Sum(e => e.VendorPrice),
                            Quantity = purchaseOrderDetails.Sum(e => e.OrderQuantity)
                    });
                    }
                    else
                    {
                        vendor.SalesAmount += purchaseOrderDetails.Sum(e => e.VendorPrice);
                        vendor.Quantity += purchaseOrderDetails.Sum(e => e.OrderQuantity);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }
        #endregion

        #region Save Data
        public async Task<bool> Create(PurchaseOrder purchaseOrder)
        {
            try
            {
                var warehouseTrackingList = new List<WarehouseTracking>();
                var orderDetailList = new List<OrderDetail>();

                // Get PFWBNumber Max + 1
                int maxPFWBNumber = _context.PurchaseOrders.Count() > 0 ? _context.PurchaseOrders.Max(e => Convert.ToInt32(e.PFWBNumber.Substring(1))) + 1 : 70000;
                purchaseOrder.PFWBNumber = $"P{maxPFWBNumber}";

                await _context.PurchaseOrders.AddAsync(purchaseOrder);
                await _context.SaveEntitiesAsync();

                // Save PO Details
                foreach (var poDetail in purchaseOrder.PurchaseOrderDetails)
                {
                    poDetail.PurchaseOrderId = purchaseOrder.Id;

                    warehouseTrackingList.Add(
                        new WarehouseTracking()
                        {
                            CreatedBy = poDetail.CreatedBy,
                            CreatedDate = poDetail.CreatedDate.ToUniversalTime(),
                            Description = "Ordered to Vendor",
                            IsActive = true,
                            IsDeleted = false,
                            OrderDetailId = poDetail.OrderDetailId,
                            OrderId = poDetail.OrderId,
                            Status = "Ordered - " + purchaseOrder.PFWBNumber
                        });

                    var orderDetail = await _context.OrderDetails.FindAsync(poDetail.OrderDetailId);
                    if (orderDetail != null)
                    {
                        orderDetail.WarehouseTracking = "Ordered - " + purchaseOrder.PFWBNumber;
                        orderDetail.StatusId = null;
                        orderDetailList.Add(orderDetail);
                    }
                }

                await _context.PurchaseOrderDetails.AddRangeAsync(purchaseOrder.PurchaseOrderDetails);
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

        public async Task<bool> Update(PurchaseOrder purchaseOrder)
        { 
            try
            {

                var orderDetailList = new List<OrderDetail>();
                var warehouseTrackingList = new List<WarehouseTracking>();
                


                // Convert Dates back to UTC prior to saving
                purchaseOrder.PurchaseOrderDate = purchaseOrder.PurchaseOrderDate.ToUniversalTime();
                purchaseOrder.CreatedDate = purchaseOrder.CreatedDate.ToUniversalTime();
                _context.PurchaseOrders.Update(purchaseOrder);

                foreach (var purchaseOrderDetail in purchaseOrder.PurchaseOrderDetails)
                {
                    var oldPOD = await _context.PurchaseOrderDetails.FirstOrDefaultAsync(x => x.Id == purchaseOrderDetail.Id);
                    if (oldPOD != null)
                    {
                        if (purchaseOrderDetail.StatusId != oldPOD.StatusId)
                        {
                            oldPOD.StatusId = purchaseOrderDetail.StatusId;
                            oldPOD.PODetailStatus = purchaseOrderDetail.PODetailStatus;
                            oldPOD.VendorPrice = purchaseOrderDetail.VendorPrice;
                            if (purchaseOrderDetail.ReceivedDate != null)
                            {
                                oldPOD.ReceivedDate = purchaseOrderDetail.ReceivedDate.Value.ToUniversalTime();
                            }

                            warehouseTrackingList.Add(
                                new WarehouseTracking()
                                {
                                    CreatedBy = purchaseOrderDetail.ModifiedBy,
                                    CreatedDate = purchaseOrderDetail.ModifiedDate.Value.ToUniversalTime(),
                                    Description = purchaseOrderDetail.PODetailStatus,
                                    IsActive = true,
                                    IsDeleted = false,
                                    OrderDetailId = purchaseOrderDetail.OrderDetailId,
                                    OrderId = purchaseOrderDetail.OrderId,
                                    Status = purchaseOrderDetail.PODetailStatus
                                });

                            _context.PurchaseOrderDetails.Update(oldPOD);
                        }
                        else if (purchaseOrderDetail.VendorPrice != oldPOD.VendorPrice)
                        {
                            oldPOD.VendorPrice = purchaseOrderDetail.VendorPrice;
                            _context.PurchaseOrderDetails.Update(oldPOD);
                        }
                    }
                }

                foreach (var poDetail in purchaseOrder.PurchaseOrderDetails)
                {
                    var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(e => e.Id == poDetail.OrderDetailId);
                    if (orderDetail != null)
                    {
                        orderDetail.VendorPrice = poDetail.VendorPrice;
                        orderDetailList.Add(orderDetail);
                    }
                }

                _context.OrderDetails.UpdateRange(orderDetailList);
                await _context.WarehouseTrackings.AddRangeAsync(warehouseTrackingList);
                await _context.SaveEntitiesAsync();
                return true;
            }
            catch ( Exception ex )
            {
                return false;
            }
        }

        public async Task<bool> SoftDeletePurchaseOrderDetail(PurchaseOrderDetail purchaseOrderDetail)
        {
            try
            {
                purchaseOrderDetail.IsDeleted = true;
                _context.PurchaseOrderDetails.Update(purchaseOrderDetail);
                
                var warehouseTracking = await _context.WarehouseTrackings.FirstOrDefaultAsync(e => e.OrderId == purchaseOrderDetail.OrderId && 
                    e.OrderDetailId == purchaseOrderDetail.OrderDetailId && e.Status.Contains("Ordered") && e.IsActive && !e.IsDeleted);

                if (warehouseTracking != null)
                {
                    warehouseTracking.IsDeleted = true;
                    _context.WarehouseTrackings.Update(warehouseTracking);
                }

                var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(e => e.Id == purchaseOrderDetail.OrderDetailId);

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

        public async Task<bool> Delete(PurchaseOrder purchaseOrder)
        {
            try
            {
                purchaseOrder.IsDeleted = true;
                _context.PurchaseOrders.Update(purchaseOrder);

                foreach (var purchaseOrderDetail in purchaseOrder.PurchaseOrderDetails) 
                { 
                    purchaseOrderDetail.IsDeleted = false; 
                }

                _context.PurchaseOrderDetails.UpdateRange(purchaseOrder.PurchaseOrderDetails);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task<List<PurchaseOrder>> SoftDelete(List<int> purchaseOrderIds)
        {
            var purchaseOrders = _context.PurchaseOrders.Where(a => purchaseOrderIds.Contains(a.Id)).ToList();
            purchaseOrders.ForEach(a => { a.IsDeleted = true; });

            _context.PurchaseOrders.UpdateRange(purchaseOrders);
            await _context.SaveEntitiesAsync();
            return await _context.PurchaseOrders.ToListAsync();
        }
        #endregion
    }
}
