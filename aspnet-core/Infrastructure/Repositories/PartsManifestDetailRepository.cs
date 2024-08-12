using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class PartsManifestDetailRepository : IPartsManifestDetailRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public PartsManifestDetailRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<PartsManifestDetail>> GetPartsManifestDetails()
        {
            return await _context.PartsManifestDetails.ToListAsync();
        }

        public async Task<PartsManifestDetail?> GetPartsManifestDetail(int partsManifestDetailId)
        {
            var result = await _context.PartsManifestDetails.FindAsync(partsManifestDetailId);
            if (result == null)
                return null;

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<PartsManifestDetail>> Create(PartsManifestDetail partsManifestDetail)
        {
            _context.PartsManifestDetails.Add(partsManifestDetail);
            await _context.SaveEntitiesAsync();
            return await _context.PartsManifestDetails.ToListAsync();
        }

        public async Task<List<PartsManifestDetail>> Update(PartsManifestDetail partsManifestDetail)
        {
            // NOTE: This is mainly used for Retry PartsManifestDetail
            
            //// Get Order and change the status back to Open
            //var order = await _context.Orders.FirstOrDefaultAsync(e => e.Id == partsManifestDetail.OrderId && e.OrderStatusId != 2);
            //if (order != null)
            //{
            //    if (order.OrderStatusId != 9)
            //    {
            //        order.OrderStatusId = 1;
            //        order.OrderStatusName = "Open";
            //        _context.Orders.Update(order);
            //    }
            //}

            // Convert Dates back to UTC prior to saving
            partsManifestDetail.CreatedDate = partsManifestDetail.CreatedDate.ToUniversalTime();
            _context.PartsManifestDetails.Update(partsManifestDetail);

            var warehouseTracking = new WarehouseTracking()
            {
                CreatedBy = partsManifestDetail.ModifiedBy != null ? partsManifestDetail.ModifiedBy : "",
                CreatedDate = partsManifestDetail.ModifiedDate != null ? partsManifestDetail.ModifiedDate.Value.ToUniversalTime() : DateTime.MinValue,
                //Description = partsManifestDetail.StatusDetail,
                Id = 0,
                IsActive = true,
                IsDeleted = false,
                //OrderDetailId = partsManifestDetail.OrderDetailId,
                //OrderId = partsManifestDetail.OrderId,
                //Status = partsManifestDetail.StatusDetail
            };

            await _context.WarehouseTrackings.AddAsync(warehouseTracking);

            await _context.SaveEntitiesAsync();
            return await _context.PartsManifestDetails.ToListAsync();
        }

        public async Task<List<PartsManifestDetail>> Delete(List<int> partsManifestDetailIds)
        {
            var partsManifestDetails = _context.PartsManifestDetails.Where(a => partsManifestDetailIds.Contains(a.Id)).ToList();
            _context.PartsManifestDetails.RemoveRange(partsManifestDetails);
            await _context.SaveEntitiesAsync();
            return await _context.PartsManifestDetails.ToListAsync();
        }

        public async Task<List<PartsManifestDetail>> SoftDelete(List<int> partsManifestDetailIds)
        {
            var partsManifestDetails = _context.PartsManifestDetails.Where(a => partsManifestDetailIds.Contains(a.Id)).ToList();
            partsManifestDetails.ForEach(a => { a.IsDeleted = true; });

            _context.PartsManifestDetails.UpdateRange(partsManifestDetails);
            await _context.SaveEntitiesAsync();
            return await _context.PartsManifestDetails.ToListAsync();
        }
        #endregion
    }
}
