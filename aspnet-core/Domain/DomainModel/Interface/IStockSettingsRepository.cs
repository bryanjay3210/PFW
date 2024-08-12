using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface IStockSettingsRepository : IRepository<StockSettings>
    {
        #region Get Data
        Task<StockSettings?> GetStockSettings();
        #endregion

        #region Save Data
        Task<bool> Update(StockSettings stockSettings);
        #endregion
    }
}
