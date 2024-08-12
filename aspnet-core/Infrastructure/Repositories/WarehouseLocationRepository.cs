using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class WarehouseLocationRepository : IWarehouseLocationRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public WarehouseLocationRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<WarehouseLocation>> GetWarehouseLocations()
        {
            return await _context.WarehouseLocations.ToListAsync();
        }

        public async Task<WarehouseLocation?> GetWarehouseLocation(int warehouseLocationId)
        {
            var result = await _context.WarehouseLocations.FindAsync(warehouseLocationId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<WarehouseLocation?> GetWarehouseLocationByLocation(int warehouseId, string location)
        {
            var result = await _context.WarehouseLocations.Where(e => e.WarehouseId == warehouseId && e.Location.Trim().ToLower() == location.Trim().ToLower()).FirstOrDefaultAsync();
            if (result == null) return null;
            return result;
        }

        public async Task<WarehouseLocationWithStockDTO?> GetWarehouseLocationByLocationWithStocks(int warehouseId, string location)
        {
            var result = new WarehouseLocationWithStockDTO();
            var warehouseLocation = await _context.WarehouseLocations.Where(e => e.WarehouseId == warehouseId && e.Location.Trim().ToLower() == location.Trim().ToLower()).FirstOrDefaultAsync();
            if (warehouseLocation != null)
            {
                result.WarehouseLocation = warehouseLocation;
                var stocks = await _context.WarehouseStocks.Where(e => e.WarehouseLocationId == warehouseLocation.Id && e.Quantity > 0).ToListAsync();
                if (stocks != null)
                {
                    foreach (var stock in stocks)
                    {
                        var product = await _context.Products.FirstOrDefaultAsync(e => e.Id == stock.ProductId);
                        if (product != null)
                        {
                            product.StockQuantity = stock.Quantity;
                            result.Stocks.Add(product);
                        }
                    }
                }

                return result;
            }
            else 
            { 
                return null; 
            }
        }

        #endregion

        #region Save Data
        public async Task<List<WarehouseLocation>> Create(WarehouseLocation warehouseLocation)
        {
            _context.WarehouseLocations.Add(warehouseLocation);
            await _context.SaveEntitiesAsync();
            return await _context.WarehouseLocations.ToListAsync();
        }

        public async Task<List<WarehousePartDTO>> CreateWithStock(WarehouseLocation warehouseLocation)
        {
            var finalResult = new List<WarehousePartDTO>();

            var warehouseStock = new WarehouseStock()
            {
                CreatedBy = warehouseLocation.CreatedBy,
                CreatedDate = warehouseLocation.CreatedDate,
                IsActive = warehouseLocation.IsActive,
                IsDeleted = warehouseLocation.IsDeleted,
                ProductId = warehouseLocation.ProductId,
                Quantity = warehouseLocation.Quantity,
                WarehouseId = warehouseLocation.WarehouseId,
                WarehouseLocationId = 0
            };

            // Get WarehouseLocation if existing
            var wl = await _context.WarehouseLocations.Where(e => e.Location.Trim().ToLower() == warehouseLocation.Location.Trim().ToLower()).FirstOrDefaultAsync();

            if (wl == null)
            {
                _context.WarehouseLocations.Add(warehouseLocation);
                await _context.SaveEntitiesAsync();

                warehouseStock.WarehouseLocationId = warehouseLocation.Id;
            }
            else
            {
                warehouseStock.WarehouseLocationId = wl.Id;
            }

            // Get WarehouseStock exists in Location
            var ws = await _context.WarehouseStocks.Where(e => e.WarehouseLocationId == warehouseStock.WarehouseLocationId && e.ProductId == warehouseStock.ProductId).FirstOrDefaultAsync();

            if (ws == null)
            {
                await _context.WarehouseStocks.AddAsync(warehouseStock);
                
            }
            else
            {
                ws.Quantity += warehouseStock.Quantity;
                _context.WarehouseStocks.Update(ws);
            }

            await _context.SaveEntitiesAsync();

            var result = await _context.WarehouseStocks.Where(e => e.ProductId == warehouseLocation.ProductId).ToListAsync();
            foreach (var item in result)
            {
                finalResult.Add(new WarehousePartDTO()
                {
                    WarehouseId = item.WarehouseId,
                    Height = 0,
                    Location = "",
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    WarehouseLocationId = item.WarehouseLocationId,
                    WarehouseStockId = item.Id,
                    Zoning = 0,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                });
            }

            foreach (var item in finalResult) 
            {
                var wLocation = await _context.WarehouseLocations.Where(e => e.Id == item.WarehouseLocationId).FirstOrDefaultAsync();
                item.Height = wLocation != null ? wLocation.Height : 0;
                item.Location = wLocation != null ? wLocation.Location : string.Empty;
                item.Zoning = wLocation != null ? wLocation.Zoning : 0;
            }

            return finalResult;
        }

        public async Task<List<WarehouseLocation>> Update(WarehouseLocation warehouseLocation)
        {
            _context.WarehouseLocations.Update(warehouseLocation);
            await _context.SaveEntitiesAsync();
            return await _context.WarehouseLocations.ToListAsync();
        }

        public async Task<List<WarehousePartDTO>> UpdateWithStock(WarehouseLocation warehouseLocation)
        {
            var finalResult = new List<WarehousePartDTO>();
            
            try
            {
                // Get existing WarehouseLocation
                var wl = await _context.WarehouseLocations.FirstOrDefaultAsync(e => e.Id == warehouseLocation.Id);

                // Validate if Location name changed
                if (wl.Location.Trim().ToLower() == warehouseLocation.Location.Trim().ToLower())
                {
                    wl.Location = warehouseLocation.Location;
                    wl.Zoning = warehouseLocation.Zoning;
                    wl.Height = warehouseLocation.Height;
                    _context.WarehouseLocations.Update(wl);

                    // Find existing stock
                    var existingStk = await _context.WarehouseStocks.FirstOrDefaultAsync(e => e.WarehouseLocationId == wl.Id && e.ProductId == warehouseLocation.ProductId);
                    if (existingStk != null)
                    {
                        // Update existing stock
                        existingStk.Quantity = warehouseLocation.Quantity;
                        _context.WarehouseStocks.Update(existingStk);
                    }
                    else
                    {
                        // Create stock
                        var warehouseStock = new WarehouseStock()
                        {
                            CreatedBy = warehouseLocation.CreatedBy,
                            CreatedDate = warehouseLocation.CreatedDate,
                            IsActive = warehouseLocation.IsActive,
                            IsDeleted = warehouseLocation.IsDeleted,
                            ProductId = warehouseLocation.ProductId,
                            Quantity = warehouseLocation.Quantity,
                            WarehouseId = warehouseLocation.WarehouseId,
                            WarehouseLocationId = warehouseLocation.Id
                        };
                        _context.WarehouseStocks.Add(warehouseStock);
                    }
                }
                else
                {
                    // Find existing Location name (This means that stock is moved to another location)
                    var existingWL = await _context.WarehouseLocations.FirstOrDefaultAsync(e => e.Location.Trim().ToLower() == warehouseLocation.Location.Trim().ToLower());
                    if (existingWL != null)
                    {
                        // For verification
                        //existingWL.Zoning = warehouseLocation.Zoning;
                        //existingWL.Height = warehouseLocation.Height;
                        //_context.WarehouseLocations.Update(existingWL);

                        // Find existing stock
                        var existingStk = await _context.WarehouseStocks.FirstOrDefaultAsync(e => e.WarehouseLocationId == existingWL.Id && e.ProductId == warehouseLocation.ProductId);
                        if (existingStk != null)
                        {
                            // Update existing stock
                            existingStk.Quantity += warehouseLocation.Quantity;
                            _context.WarehouseStocks.Update(existingStk);
                        }
                        else
                        {
                            // Create stock
                            var warehouseStock = new WarehouseStock()
                            {
                                CreatedBy = warehouseLocation.CreatedBy,
                                CreatedDate = warehouseLocation.CreatedDate,
                                IsActive = warehouseLocation.IsActive,
                                IsDeleted = warehouseLocation.IsDeleted,
                                ProductId = warehouseLocation.ProductId,
                                Quantity = warehouseLocation.Quantity,
                                WarehouseId = warehouseLocation.WarehouseId,
                                WarehouseLocationId = existingWL.Id
                            };
                            _context.WarehouseStocks.Add(warehouseStock);
                        }

                        // Update original Location stock quantity to zero
                        var originalStk = await _context.WarehouseStocks.FirstOrDefaultAsync(e => e.WarehouseLocationId == warehouseLocation.Id && e.ProductId == warehouseLocation.ProductId);
                        if (originalStk != null)
                        {
                            originalStk.Quantity = 0;
                            _context.WarehouseStocks.Update(originalStk);
                        }
                    }
                    else
                    {
                        // Create WarehouseLocation
                        var newWL = new WarehouseLocation()
                        {
                            CreatedBy = warehouseLocation.ModifiedBy,
                            CreatedDate = warehouseLocation.ModifiedDate.Value,
                            IsActive = true,
                            IsDeleted = false,
                            Sequence = 0,
                            WarehouseId = 1,
                            Location = warehouseLocation.Location,
                            Zoning = warehouseLocation.Zoning,
                            Height = warehouseLocation.Height
                        };

                        _context.WarehouseLocations.Add(newWL);
                        await _context.SaveEntitiesAsync();

                        // Create WarehouseStock
                        var warehouseStock = new WarehouseStock()
                        {
                            CreatedBy = warehouseLocation.CreatedBy,
                            CreatedDate = warehouseLocation.CreatedDate,
                            IsActive = warehouseLocation.IsActive,
                            IsDeleted = warehouseLocation.IsDeleted,
                            ProductId = warehouseLocation.ProductId,
                            Quantity = warehouseLocation.Quantity,
                            WarehouseId = warehouseLocation.WarehouseId,
                            WarehouseLocationId = newWL.Id
                        };
                        _context.WarehouseStocks.Add(warehouseStock);
                    }
                }

                await _context.SaveEntitiesAsync();

                var result = await _context.WarehouseStocks.Where(e => e.ProductId == warehouseLocation.ProductId).ToListAsync();
                foreach (var item in result)
                {
                    finalResult.Add(new WarehousePartDTO()
                    {
                        WarehouseId = item.WarehouseId,
                        Height = 0,
                        Location = "",
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        WarehouseLocationId = item.WarehouseLocationId,
                        WarehouseStockId = item.Id,
                        Zoning = 0,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate
                    });
                }

                foreach (var item in finalResult)
                {
                    var wLocation = await _context.WarehouseLocations.Where(e => e.Id == item.WarehouseLocationId).FirstOrDefaultAsync();
                    item.Height = wLocation != null ? wLocation.Height : 0;
                    item.Location = wLocation != null ? wLocation.Location : string.Empty;
                    item.Zoning = wLocation != null ? wLocation.Zoning : 0;
                }

                return finalResult;
            }
            catch (Exception ex)
            {
                return finalResult;
            }
        }

        public async Task<List<WarehouseLocation>> Delete(List<int> warehouseLocationIds)
        {
            var warehouseLocations = _context.WarehouseLocations.Where(a => warehouseLocationIds.Contains(a.Id)).ToList();
            _context.WarehouseLocations.RemoveRange(warehouseLocations);
            await _context.SaveEntitiesAsync();
            return await _context.WarehouseLocations.ToListAsync();
        }

        public async Task<List<WarehouseLocation>> SoftDelete(List<int> warehouseLocationIds)
        {
            var warehouseLocations = _context.WarehouseLocations.Where(a => warehouseLocationIds.Contains(a.Id)).ToList();
            warehouseLocations.ForEach(c => { c.IsDeleted = true; });

            _context.WarehouseLocations.UpdateRange(warehouseLocations);
            await _context.SaveEntitiesAsync();
            return await _context.WarehouseLocations.ToListAsync();
        }

        #endregion
    }
}
