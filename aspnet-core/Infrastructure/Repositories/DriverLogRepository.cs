using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO.Paginated;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class DriverLogRepository : IDriverLogRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public DriverLogRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<DriverLog>> GetDriverLogs()
        {
            return await _context.DriverLogs.ToListAsync();
        }

        public async Task<DriverLog?> GetDriverLog(int driverLogId)
        {
            var result = await _context.DriverLogs.FindAsync(driverLogId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<PaginatedListDTO<DriverLog>> GetDriverLogsPaginated(int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search)
        {
            var recordCount = (
                from driverLog in _context.DriverLogs
                where driverLog.IsActive == true && driverLog.IsDeleted == false &&
                    (string.IsNullOrEmpty(search) ? true : driverLog.DriverName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : driverLog.DriverLogNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : driverLog.StatusDetail.Trim().ToLower().Contains(search) 
                     //|| string.IsNullOrEmpty(search) ? true : driverLog.CreatedDate.ToString("MM/dd/yyyy").Contains(search)
                     )
                select driverLog)
                .Distinct()
                .Count();

            var driverLogs = await (
                from driverLog in _context.DriverLogs
                where driverLog.IsActive == true && driverLog.IsDeleted == false &&
                    (string.IsNullOrEmpty(search) ? true : driverLog.DriverName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : driverLog.DriverLogNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : driverLog.StatusDetail.Trim().ToLower().Contains(search)
                     //|| string.IsNullOrEmpty(search) ? true : driverLog.CreatedDate.ToString("MM/dd/yyyy").Contains(search)
                     )
                select driverLog)
                .Distinct()
                .OrderByDescending(e => e.Id)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            foreach (var driverLog in driverLogs)
            {
                var driverLogDetails = await _context.DriverLogDetails.Where(e => e.DriverLogId == driverLog.Id && e.IsActive && !e.IsDeleted).ToListAsync();
                // Get distinct Order Ids
                if (driverLogDetails != null)
                {
                    List<int> orderIds = driverLogDetails.DistinctBy(e => e.OrderId).Select(f => f.OrderId).ToList();
                    
                    foreach ( var orderId in orderIds)
                    {
                        // Get Totals per Order
                        var totalAmount = driverLogDetails.Where(e => e.OrderId == orderId).Sum(e => e.TotalAmount);
                        
                        if (driverLogDetails.FirstOrDefault(e => e.OrderId == orderId) != null)
                        {
                            driverLogDetails.FirstOrDefault(e => e.OrderId == orderId).OrderTotalAmount = totalAmount;
                        }
                        
                    }
                    
                    driverLog.DriverLogDetails = driverLogDetails;
                }
            }

            var result = new PaginatedListDTO<DriverLog>()
            {
                Data = driverLogs,
                RecordCount = recordCount
            };

            return result;
        }

        public async Task<PaginatedListDTO<DriverLog>> GetDriverLogsByDatePaginated(int pageSize, int pageIndex, DateTime fromDate, DateTime toDate)
        {
            DateTime fDate = fromDate.ToUniversalTime();
            DateTime tDate = toDate.ToUniversalTime();

            var recordCount = (
                from driverLog in _context.DriverLogs
                where driverLog.IsActive == true && driverLog.IsDeleted == false && 
                    (driverLog.CreatedDate.Date >= fDate.Date && driverLog.CreatedDate.Date <= tDate.Date)
                select driverLog)
                .Distinct()
                .Count();

            var driverLogs = await (
                from driverLog in _context.DriverLogs
                where driverLog.IsActive == true && driverLog.IsDeleted == false && 
                    (driverLog.CreatedDate.Date >= fDate.Date && driverLog.CreatedDate.Date <= tDate.Date)
                select driverLog)
                .Distinct()
                .OrderByDescending(e => e.CreatedDate)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            foreach (var driverLog in driverLogs)
            {
                var driverLogDetails = await _context.DriverLogDetails.Where(e => e.DriverLogId == driverLog.Id && e.IsActive && !e.IsDeleted).ToListAsync();
                // Get distinct Order Ids
                if (driverLogDetails != null)
                {
                    List<int> orderIds = driverLogDetails.DistinctBy(e => e.OrderId).Select(f => f.OrderId).ToList();

                    foreach (var orderId in orderIds)
                    {
                        // Get Totals per Order
                        var totalAmount = driverLogDetails.Where(e => e.OrderId == orderId).Sum(e => e.TotalAmount);

                        if (driverLogDetails.FirstOrDefault(e => e.OrderId == orderId) != null)
                        {
                            driverLogDetails.FirstOrDefault(e => e.OrderId == orderId).OrderTotalAmount = totalAmount;
                        }
                    }

                    driverLog.DriverLogDetails = driverLogDetails;
                }
            }

            var result = new PaginatedListDTO<DriverLog>()
            {
                Data = driverLogs,
                RecordCount = recordCount
            };

            return result;
        }
        #endregion

        #region Save Data
        public async Task<bool> Create(DriverLog driverLog)
        {
            try
            {
                var trackingStatus = $"DELIVERED BY {driverLog.DriverName}";

                // Get PFWBNumber Max + 1
                int maxDriverLogNumber = _context.DriverLogs.Count() > 0 ? _context.DriverLogs.Max(e => Convert.ToInt32(e.DriverLogNumber.Substring(2))) + 1 : 0;
                driverLog.DriverLogNumber = $"DL{maxDriverLogNumber.ToString("D4")}";

                await _context.DriverLogs.AddAsync(driverLog);
                await _context.SaveEntitiesAsync();


                // Update Order status
                List<int> orderIds = driverLog.DriverLogDetails.DistinctBy(e => e.OrderId).Select(f => f.OrderId).ToList();

                foreach (var orderId in orderIds)
                {
                    var order = await _context.Orders.FirstOrDefaultAsync(e => e.Id == orderId);
                    if (order != null && order.OrderStatusId != 2 && order.OrderStatusId != 9)
                    {
                        order.OrderStatusId = 4;
                        order.OrderStatusName = "Delivered";
                        _context.Orders.Update(order);
                    }
                }


                foreach (var driverLogDetail in driverLog.DriverLogDetails)
                {
                    driverLogDetail.DriverLogId = driverLog.Id;
                    await _context.DriverLogDetails.AddAsync(driverLogDetail);

                    // Add warehouse Tracking
                    var warehouseTracking = new WarehouseTracking()
                    {
                        CreatedBy = driverLog.CreatedBy,
                        CreatedDate = driverLog.CreatedDate.ToUniversalTime(),
                        Description = trackingStatus,
                        Id = 0,
                        IsActive = true,
                        IsDeleted = false,
                        OrderDetailId = driverLogDetail.OrderDetailId,
                        OrderId = driverLogDetail.OrderId,
                        Status = trackingStatus
                    };

                    await _context.WarehouseTrackings.AddAsync(warehouseTracking);

                    // Update OrderDetai WarehouseTracking
                    var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(e => e.Id == driverLogDetail.OrderDetailId);
                    if (orderDetail != null)
                    {
                        orderDetail.WarehouseTracking = trackingStatus;
                        _context.OrderDetails.Update(orderDetail);
                    }
                }

                await _context.SaveEntitiesAsync();
                return true;
            }
            catch 
            {
                return false;
            }
        }

        public async Task<List<DriverLog>> Update(DriverLog driverLog)
        {
            // Convert Dates back to UTC prior to saving
            driverLog.CreatedDate = driverLog.CreatedDate.ToUniversalTime();

            _context.DriverLogs.Update(driverLog);
            await _context.SaveEntitiesAsync();
            return await _context.DriverLogs.ToListAsync();
        }

        public async Task<List<DriverLog>> Delete(List<int> driverLogIds)
        {
            var driverLogs = _context.DriverLogs.Where(a => driverLogIds.Contains(a.Id)).ToList();
            _context.DriverLogs.RemoveRange(driverLogs);
            await _context.SaveEntitiesAsync();
            return await _context.DriverLogs.ToListAsync();
        }

        public async Task<List<DriverLog>> SoftDelete(List<int> driverLogIds)
        {
            var driverLogs = _context.DriverLogs.Where(a => driverLogIds.Contains(a.Id)).ToList();
            driverLogs.ForEach(a => { a.IsDeleted = true; });

            _context.DriverLogs.UpdateRange(driverLogs);
            await _context.SaveEntitiesAsync();
            return await _context.DriverLogs.ToListAsync();
        }
        #endregion
    }
}
