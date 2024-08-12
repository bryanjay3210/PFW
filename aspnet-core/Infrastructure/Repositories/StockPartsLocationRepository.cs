using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32.SafeHandles;

namespace Infrastucture.Repositories
{
    public class StockPartsLocationRepository : IStockPartsLocationRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public StockPartsLocationRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<StockPartsLocation>> GetStockPartsLocations()
        {
            return await _context.StockPartsLocations.ToListAsync();
        }
        public async Task<List<StockPartsLocation>> GetStockPartsLocationsByPartNumber(string partNumber)
        {
            return await _context.StockPartsLocations.Where(e => e.PartNumber == partNumber && e.IsDeleted == false).ToListAsync();
        }

        public async Task<StockPartsLocation?> GetStockPartsLocation(int stockPartsLocationId)
        {
            var result = await _context.StockPartsLocations.FindAsync(stockPartsLocationId);
            if (result == null)
                return null;

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<StockPartsLocation>> Create(StockPartsLocation stockPartsLocation)
        {
            _context.StockPartsLocations.Add(stockPartsLocation);
            await _context.SaveEntitiesAsync();
            return await _context.StockPartsLocations.ToListAsync();
        }

        public async Task<List<StockPartsLocation>> Update(StockPartsLocation stockPartsLocation)
        {
            _context.StockPartsLocations.Update(stockPartsLocation);
            await _context.SaveEntitiesAsync();
            return await _context.StockPartsLocations.ToListAsync();
        }

        public async Task<List<StockPartsLocation>> Delete(List<int> stockPartsLocationIds)
        {
            var stockPartsLocations = _context.StockPartsLocations.Where(a => stockPartsLocationIds.Contains(a.Id)).ToList();
            _context.StockPartsLocations.RemoveRange(stockPartsLocations);
            await _context.SaveEntitiesAsync();
            return await _context.StockPartsLocations.ToListAsync();
        }

        public async Task<List<StockPartsLocation>> SoftDelete(List<int> stockPartsLocationIds)
        {
            var stockPartsLocations = _context.StockPartsLocations.Where(a => stockPartsLocationIds.Contains(a.Id)).ToList();
            stockPartsLocations.ForEach(c => { c.IsDeleted = true; });

            _context.StockPartsLocations.UpdateRange(stockPartsLocations);
            await _context.SaveEntitiesAsync();
            return await _context.StockPartsLocations.ToListAsync();
        }
        #endregion
    }
}
