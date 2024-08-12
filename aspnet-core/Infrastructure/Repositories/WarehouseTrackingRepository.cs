using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Infrastucture.Repositories
{
    public class WarehouseTrackingRepository : IWarehouseTrackingRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public WarehouseTrackingRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<WarehouseTracking?> GetWarehouseTracking(int warehouseTrackingId)
        {
            var result = await _context.WarehouseTrackings.FindAsync(warehouseTrackingId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<List<WarehouseTracking>> GetWarehouseTrackings()
        {
            return await _context.WarehouseTrackings.ToListAsync();
        }

        
        #endregion

        #region Save Data
        public async Task<List<WarehouseTracking>> Create(WarehouseTracking warehouseTracking)
        {
            _context.WarehouseTrackings.Add(warehouseTracking);
            await _context.SaveEntitiesAsync();
            return await _context.WarehouseTrackings.ToListAsync();
        }

        public async Task<List<WarehouseTracking>> Update(WarehouseTracking warehouseTracking)
        {
            _context.WarehouseTrackings.Update(warehouseTracking);
            await _context.SaveEntitiesAsync();
            return await _context.WarehouseTrackings.ToListAsync();
        }

        public async Task<List<WarehouseTracking>> Delete(List<int> warehouseTrackingIds)
        {
            var warehouseTrackings = _context.WarehouseTrackings.Where(a => warehouseTrackingIds.Contains(a.Id)).ToList();
            _context.WarehouseTrackings.RemoveRange(warehouseTrackings);
            await _context.SaveEntitiesAsync();
            return await _context.WarehouseTrackings.ToListAsync();
        }

        public async Task<List<WarehouseTracking>> SoftDelete(List<int> warehouseTrackingIds)
        {
            var warehouseTrackings = _context.WarehouseTrackings.Where(a => warehouseTrackingIds.Contains(a.Id)).ToList();
            warehouseTrackings.ForEach(c => { c.IsDeleted = true; });

            _context.WarehouseTrackings.UpdateRange(warehouseTrackings);
            await _context.SaveEntitiesAsync();
            return await _context.WarehouseTrackings.ToListAsync();
        }

        #endregion
    }
}
