using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class WarehouseStockRepository : IWarehouseStockRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public WarehouseStockRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<WarehouseStock?> GetWarehouseStock(int warehouseStockId)
        {
            var result = await _context.WarehouseStocks.FindAsync(warehouseStockId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<List<WarehouseStock>> GetWarehouseStocks()
        {
            var result = await ( //_context.WarehouseStocks.ToListAsync();
                from ws in _context.WarehouseStocks
                join wl in _context.WarehouseLocations on ws.WarehouseLocationId equals wl.Id
                where ws.IsActive == true && ws.IsDeleted == false 
            select new WarehouseStock
            {
                CreatedBy = ws.CreatedBy,
                CreatedDate = ws.CreatedDate,
                IsActive = ws.IsActive,
                IsDeleted = ws.IsDeleted,
                Id = ws.Id,
                ModifiedBy = ws.ModifiedBy,
                ModifiedDate = ws.ModifiedDate,
                ProductId = ws.ProductId,
                Quantity = ws.Quantity,
                WarehouseId = ws.WarehouseId,
                WarehouseLocationId = wl.Id,
                Location = wl.Location,
            })
            .Distinct()
            .ToListAsync();

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<WarehouseStock>> Create(WarehouseStock warehouseStock)
        {
            _context.WarehouseStocks.Add(warehouseStock);
            await _context.SaveEntitiesAsync();
            return await _context.WarehouseStocks.ToListAsync();
        }

        public async Task<bool> UpdateWarehouseStocks(List<WarehouseStockDTO> warehouseStocks)
        {
            try
            {
                foreach (var wstock in warehouseStocks)
                {
                    var stock = await _context.WarehouseStocks.Where(e => e.ProductId == wstock.ProductId && e.WarehouseLocationId == wstock.WarehouseLocationId).FirstOrDefaultAsync();
                    if (stock != null)
                    {
                        stock.Quantity += wstock.Quantity;
                        stock.ModifiedBy = wstock.ModifiedBy;
                        stock.ModifiedDate = wstock.ModifiedDate;
                        _context.WarehouseStocks.Update(stock);
                    }
                    else
                    {
                        var newStock = new WarehouseStock() 
                        { 
                            CreatedBy = wstock.CreatedBy,
                            CreatedDate = wstock.CreatedDate,
                            IsActive = wstock.IsActive,
                            IsDeleted = wstock.IsDeleted,
                            ProductId = wstock.ProductId,
                            Quantity = wstock.Quantity,
                            WarehouseId = wstock.WarehouseId,
                            WarehouseLocationId = wstock.WarehouseLocationId
                        };
                        await _context.WarehouseStocks.AddAsync(newStock);
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateCycleCount(List<WarehouseStockDTO> warehouseStocks)
        {
            try
            {
                foreach (var wstock in warehouseStocks)
                {
                    var stock = await _context.WarehouseStocks.Where(e => e.ProductId == wstock.ProductId && e.WarehouseLocationId == wstock.WarehouseLocationId).FirstOrDefaultAsync();
                    if (stock != null)
                    {
                        stock.Quantity = wstock.Quantity;
                        stock.ModifiedBy = wstock.ModifiedBy;
                        stock.ModifiedDate = wstock.ModifiedDate;
                        _context.WarehouseStocks.Update(stock);
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> TransferWarehouseStocks(List<WarehouseStockDTO> warehouseStocks)
        {
            try
            {
                foreach (var wstock in warehouseStocks)
                {
                    // Deduct transfered quantity
                    var originalStock = await _context.WarehouseStocks.Where(e => e.ProductId == wstock.ProductId && e.WarehouseLocationId == wstock.WarehouseLocationId).FirstOrDefaultAsync();
                    if (originalStock != null)
                    {
                        originalStock.Quantity -= wstock.Quantity;
                        originalStock.ModifiedBy = wstock.ModifiedBy;
                        originalStock.ModifiedDate = wstock.ModifiedDate;
                        _context.WarehouseStocks.Update(originalStock);
                    }


                    var stock = await _context.WarehouseStocks.Where(e => e.ProductId == wstock.ProductId && e.WarehouseLocationId == wstock.DestinationWarehouseLocationId).FirstOrDefaultAsync();
                    if (stock != null)
                    {
                        stock.Quantity += wstock.Quantity;
                        stock.ModifiedBy = wstock.ModifiedBy;
                        stock.ModifiedDate = wstock.ModifiedDate;
                        _context.WarehouseStocks.Update(stock);
                    }
                    else
                    {
                        var newStock = new WarehouseStock()
                        {
                            CreatedBy = wstock.CreatedBy,
                            CreatedDate = wstock.CreatedDate,
                            IsActive = wstock.IsActive,
                            IsDeleted = wstock.IsDeleted,
                            Location = wstock.DestinationLocation != null ? wstock.DestinationLocation : "",
                            ProductId = wstock.ProductId,
                            Quantity = wstock.Quantity,
                            WarehouseId = wstock.WarehouseId,
                            WarehouseLocationId = wstock.DestinationWarehouseLocationId != null ? wstock.DestinationWarehouseLocationId.Value : 0
                        };
                        await _context.WarehouseStocks.AddAsync(newStock);
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<WarehouseStock>> Update(WarehouseStock warehouseStock)
        {
            _context.WarehouseStocks.Update(warehouseStock);
            await _context.SaveEntitiesAsync();
            return await _context.WarehouseStocks.ToListAsync();
        }

        public async Task<List<WarehouseStock>> Delete(List<int> warehouseStockIds)
        {
            var warehouseStocks = _context.WarehouseStocks.Where(a => warehouseStockIds.Contains(a.Id)).ToList();
            _context.WarehouseStocks.RemoveRange(warehouseStocks);
            await _context.SaveEntitiesAsync();
            return await _context.WarehouseStocks.ToListAsync();
        }

        public async Task<List<WarehouseStock>> SoftDelete(List<int> warehouseStockIds)
        {
            var warehouseStocks = _context.WarehouseStocks.Where(a => warehouseStockIds.Contains(a.Id)).ToList();
            warehouseStocks.ForEach(c => { c.IsDeleted = true; });

            _context.WarehouseStocks.UpdateRange(warehouseStocks);
            await _context.SaveEntitiesAsync();
            return await _context.WarehouseStocks.ToListAsync();
        }

        #endregion
    }
}
