using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Infrastucture.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public WarehouseRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<Warehouse?> GetWarehouse(int warehouseId)
        {
            var result = await _context.Warehouses.FindAsync(warehouseId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<List<Warehouse>> GetWarehouses()
        {
            return await _context.Warehouses.ToListAsync();
        }

        public async Task<List<WarehousePartDTO>> GetWarehousePartsByProductId(int productId)
        {
            var warehousePartsList = new List<WarehousePartDTO>();
            var warehouseStocks = await (
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
                    Height = loc.Result != null ? loc.Result.Height: 0,
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
        public async Task<List<Warehouse>> Create(Warehouse warehouse)
        {
            _context.Warehouses.Add(warehouse);
            await _context.SaveEntitiesAsync();
            return await _context.Warehouses.ToListAsync();
        }

        public async Task<List<Warehouse>> Update(Warehouse warehouse)
        {
            _context.Warehouses.Update(warehouse);
            await _context.SaveEntitiesAsync();
            return await _context.Warehouses.ToListAsync();
        }

        public async Task<List<Warehouse>> Delete(List<int> warehouseIds)
        {
            var warehouses = _context.Warehouses.Where(a => warehouseIds.Contains(a.Id)).ToList();
            _context.Warehouses.RemoveRange(warehouses);
            await _context.SaveEntitiesAsync();
            return await _context.Warehouses.ToListAsync();
        }

        public async Task<List<Warehouse>> SoftDelete(List<int> warehouseIds)
        {
            var warehouses = _context.Warehouses.Where(a => warehouseIds.Contains(a.Id)).ToList();
            warehouses.ForEach(c => { c.IsDeleted = true; });

            _context.Warehouses.UpdateRange(warehouses);
            await _context.SaveEntitiesAsync();
            return await _context.Warehouses.ToListAsync();
        }

        #endregion
    }
}
