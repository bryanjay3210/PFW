using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface IStockPartsLocationRepository : IRepository<StockPartsLocation>
    {
        #region Get Data
        Task<List<StockPartsLocation>> GetStockPartsLocations();
        Task<List<StockPartsLocation>> GetStockPartsLocationsByPartNumber(string partNumber);
        Task<StockPartsLocation?> GetStockPartsLocation(int stockPartsLocationId);
        #endregion

        #region Save Data
        Task<List<StockPartsLocation>> Create(StockPartsLocation stockPartsLocation);
        Task<List<StockPartsLocation>> Update(StockPartsLocation stockPartsLocation);
        Task<List<StockPartsLocation>> Delete(List<int> stockPartsLocationIds);
        Task<List<StockPartsLocation>> SoftDelete(List<int> stockPartsLocationIds);
        #endregion
    }
}
