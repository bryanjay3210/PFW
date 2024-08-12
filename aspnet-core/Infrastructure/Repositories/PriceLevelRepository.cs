using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class PriceLevelRepository : IPriceLevelRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public PriceLevelRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<PriceLevel?> GetPriceLevel(int priceLevelId)
        {
            var result = await _context.PriceLevels.FindAsync(priceLevelId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<List<PriceLevel>> GetPriceLevels()
        {
            return await _context.PriceLevels.ToListAsync();
        }
        #endregion

        #region Save Data
        public async Task<List<PriceLevel>> Create(PriceLevel priceLevel)
        {
            _context.PriceLevels.Add(priceLevel);
            await _context.SaveEntitiesAsync();
            return await _context.PriceLevels.ToListAsync();
        }

        public async Task<List<PriceLevel>> Update(PriceLevel priceLevel)
        {
            _context.PriceLevels.Update(priceLevel);
            await _context.SaveEntitiesAsync();
            return await _context.PriceLevels.ToListAsync();
        }

        public async Task<List<PriceLevel>> Delete(List<int> priceLevelIds)
        {
            var priceLevels = _context.PriceLevels.Where(a => priceLevelIds.Contains(a.Id)).ToList();
            _context.PriceLevels.RemoveRange(priceLevels);
            await _context.SaveEntitiesAsync();
            return await _context.PriceLevels.ToListAsync();
        }

        public async Task<List<PriceLevel>> SoftDelete(List<int> priceLevelIds)
        {
            var priceLevels = _context.PriceLevels.Where(a => priceLevelIds.Contains(a.Id)).ToList();
            priceLevels.ForEach(c => { c.IsDeleted = true; });

            _context.PriceLevels.UpdateRange(priceLevels);
            await _context.SaveEntitiesAsync();
            return await _context.PriceLevels.ToListAsync();
        }
        #endregion
    }
}
