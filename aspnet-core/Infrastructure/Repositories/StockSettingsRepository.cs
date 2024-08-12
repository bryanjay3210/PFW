using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class StockSettingsRepository : IStockSettingsRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public StockSettingsRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<StockSettings?> GetStockSettings()
        {
            return await _context.StockSettings.FirstOrDefaultAsync();
        }
        #endregion

        #region Save Data
        public async Task<bool> Update(StockSettings stockSettings)
        {
            try
            {
                _context.StockSettings.Update(stockSettings);
                await _context.SaveEntitiesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
