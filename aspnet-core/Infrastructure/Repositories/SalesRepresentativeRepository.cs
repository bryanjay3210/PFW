using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class SalesRepresentativeRepository : ISalesRepresentativeRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public SalesRepresentativeRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<SalesRepresentative?> GetSalesRepresentative(int salesRepresentativeId)
        {
            var result = await _context.SalesRepresentatives.FindAsync(salesRepresentativeId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<List<SalesRepresentative>> GetSalesRepresentatives()
        {
            return await _context.SalesRepresentatives.ToListAsync();
        }
        #endregion

        #region Save Data
        public async Task<List<SalesRepresentative>> Create(SalesRepresentative salesRepresentative)
        {
            _context.SalesRepresentatives.Add(salesRepresentative);
            await _context.SaveEntitiesAsync();
            return await _context.SalesRepresentatives.ToListAsync();
        }

        public async Task<List<SalesRepresentative>> Update(SalesRepresentative salesRepresentative)
        {
            _context.SalesRepresentatives.Update(salesRepresentative);
            await _context.SaveEntitiesAsync();
            return await _context.SalesRepresentatives.ToListAsync();
        }

        public async Task<List<SalesRepresentative>> Delete(List<int> salesRepresentativeIds)
        {
            var salesRepresentatives = _context.SalesRepresentatives.Where(a => salesRepresentativeIds.Contains(a.Id)).ToList();
            _context.SalesRepresentatives.RemoveRange(salesRepresentatives);
            await _context.SaveEntitiesAsync();
            return await _context.SalesRepresentatives.ToListAsync();
        }

        public async Task<List<SalesRepresentative>> SoftDelete(List<int> salesRepresentativeIds)
        {
            var salesRepresentatives = _context.SalesRepresentatives.Where(a => salesRepresentativeIds.Contains(a.Id)).ToList();
            salesRepresentatives.ForEach(c => { c.IsDeleted = true; });

            _context.SalesRepresentatives.UpdateRange(salesRepresentatives);
            await _context.SaveEntitiesAsync();
            return await _context.SalesRepresentatives.ToListAsync();
        }
        #endregion
    }
}
