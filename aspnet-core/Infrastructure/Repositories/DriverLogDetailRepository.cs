using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class DriverLogDetailRepository : IDriverLogDetailRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public DriverLogDetailRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<DriverLogDetail>> GetDriverLogDetails()
        {
            return await _context.DriverLogDetails.ToListAsync();
        }

        public async Task<DriverLogDetail?> GetDriverLogDetail(int driverLogDetailId)
        {
            var result = await _context.DriverLogDetails.FindAsync(driverLogDetailId);
            if (result == null)
                return null;

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<DriverLogDetail>> Create(DriverLogDetail driverLogDetail)
        {
            _context.DriverLogDetails.Add(driverLogDetail);
            await _context.SaveEntitiesAsync();
            return await _context.DriverLogDetails.ToListAsync();
        }

        public async Task<List<DriverLogDetail>> Update(DriverLogDetail driverLogDetail)
        {
            // NOTE: This is mainly used for Retry DriverLogDetail
            
            // Get Order and change the status back to Open
            var order = await _context.Orders.FirstOrDefaultAsync(e => e.Id == driverLogDetail.OrderId && e.OrderStatusId != 2);
            if (order != null)
            {
                if (order.OrderStatusId != 9)
                {
                    order.OrderStatusId = 1;
                    order.OrderStatusName = "Open";
                    _context.Orders.Update(order);
                }
            }

            //// Get OrderDetail and change statusId to 6 - Retry
            //var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(e => e.Id == driverLogDetail.OrderDetailId);
            //if (orderDetail != null)
            //{
            //    orderDetail.StatusId = 6;
            //    _context.OrderDetails.Update(orderDetail);
            //}

            // Convert Dates back to UTC prior to saving
            driverLogDetail.CreatedDate = driverLogDetail.CreatedDate.ToUniversalTime();
            _context.DriverLogDetails.Update(driverLogDetail);

            var warehouseTracking = new WarehouseTracking()
            {
                CreatedBy = driverLogDetail.ModifiedBy != null ? driverLogDetail.ModifiedBy : "",
                CreatedDate = driverLogDetail.ModifiedDate != null ? driverLogDetail.ModifiedDate.Value.ToUniversalTime() : DateTime.MinValue,
                Description = driverLogDetail.StatusDetail,
                Id = 0,
                IsActive = true,
                IsDeleted = false,
                OrderDetailId = driverLogDetail.OrderDetailId,
                OrderId = driverLogDetail.OrderId,
                Status = driverLogDetail.StatusDetail
            };

            await _context.WarehouseTrackings.AddAsync(warehouseTracking);

            await _context.SaveEntitiesAsync();
            return await _context.DriverLogDetails.ToListAsync();
        }

        public async Task<List<DriverLogDetail>> Delete(List<int> driverLogDetailIds)
        {
            var driverLogDetails = _context.DriverLogDetails.Where(a => driverLogDetailIds.Contains(a.Id)).ToList();
            _context.DriverLogDetails.RemoveRange(driverLogDetails);
            await _context.SaveEntitiesAsync();
            return await _context.DriverLogDetails.ToListAsync();
        }

        public async Task<List<DriverLogDetail>> SoftDelete(List<int> driverLogDetailIds)
        {
            var driverLogDetails = _context.DriverLogDetails.Where(a => driverLogDetailIds.Contains(a.Id)).ToList();
            driverLogDetails.ForEach(a => { a.IsDeleted = true; });

            _context.DriverLogDetails.UpdateRange(driverLogDetails);
            await _context.SaveEntitiesAsync();
            return await _context.DriverLogDetails.ToListAsync();
        }
        #endregion
    }
}
