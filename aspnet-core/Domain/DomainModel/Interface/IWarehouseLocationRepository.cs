using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;

namespace Domain.DomainModel.Interface
{
    public interface IWarehouseLocationRepository : IRepository<WarehouseLocation>
    {
        #region Get Data
        Task<List<WarehouseLocation>> GetWarehouseLocations();
        Task<WarehouseLocation?> GetWarehouseLocation(int warehouseLocationId);
        Task<WarehouseLocation?> GetWarehouseLocationByLocation(int warehouseId, string location);
        Task<WarehouseLocationWithStockDTO?> GetWarehouseLocationByLocationWithStocks(int warehouseId, string location);
        #endregion

        #region Save Data
        Task<List<WarehouseLocation>> Create(WarehouseLocation warehouseLocation);
        Task<List<WarehousePartDTO>> CreateWithStock(WarehouseLocation warehouseLocation);
        Task<List<WarehouseLocation>> Update(WarehouseLocation warehouseLocation);
        Task<List<WarehousePartDTO>> UpdateWithStock(WarehouseLocation warehouseLocation);
        Task<List<WarehouseLocation>> Delete(List<int> warehouseLocationIds);
        Task<List<WarehouseLocation>> SoftDelete(List<int> warehouseLocationIds);
        #endregion
    }
}
