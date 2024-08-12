using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class AutomobileRepository : IAutomobileRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public AutomobileRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<Automobile>> GetAutomobiles()
        {
            return await _context.Automobiles.ToListAsync();
        }

        public async Task<Automobile?> GetAutomobile(int automobileId)
        {
            var result = await _context.Automobiles.FindAsync(automobileId);
            if (result == null)
                return null;

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<Automobile>> Create(Automobile automobile)
        {
            _context.Automobiles.Add(automobile);
            await _context.SaveEntitiesAsync();
            return await _context.Automobiles.ToListAsync();
        }

        public async Task<List<Automobile>> Update(Automobile automobile)
        {
            _context.Automobiles.Update(automobile);
            await _context.SaveEntitiesAsync();
            return await _context.Automobiles.ToListAsync();
        }

        public async Task<List<Automobile>> Delete(List<int> automobileIds)
        {
            var automobiles = _context.Automobiles.Where(a => automobileIds.Contains(a.Id)).ToList();
            _context.Automobiles.RemoveRange(automobiles);
            await _context.SaveEntitiesAsync();
            return await _context.Automobiles.ToListAsync();
        }

        public async Task<List<Automobile>> SoftDelete(List<int> automobileIds)
        {
            var automobiles = _context.Automobiles.Where(a => automobileIds.Contains(a.Id)).ToList();
            automobiles.ForEach(a => { a.IsDeleted = true; });

            _context.Automobiles.UpdateRange(automobiles);
            await _context.SaveEntitiesAsync();
            return await _context.Automobiles.ToListAsync();
        }
        #endregion
    }
}
