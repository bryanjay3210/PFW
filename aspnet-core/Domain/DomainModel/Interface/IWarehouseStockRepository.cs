using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;

namespace Domain.DomainModel.Interface
{
    public interface IWarehouseStockRepository : IRepository<WarehouseStock>
    {
        #region Get Data
        Task<List<WarehouseStock>> GetWarehouseStocks();
        Task<WarehouseStock?> GetWarehouseStock(int warehouseStockId);
        #endregion

        #region Save Data
        Task<List<WarehouseStock>> Create(WarehouseStock warehouseStock);
        Task<bool> UpdateWarehouseStocks(List<WarehouseStockDTO> warehouseStocks);
        Task<bool> UpdateCycleCount(List<WarehouseStockDTO> warehouseStocks);
        Task<bool> TransferWarehouseStocks(List<WarehouseStockDTO> warehouseStocks);
        Task<List<WarehouseStock>> Update(WarehouseStock warehouseStock);
        Task<List<WarehouseStock>> Delete(List<int> warehouseStockIds);
        Task<List<WarehouseStock>> SoftDelete(List<int> warehouseStockIds);
        #endregion
    }
}
