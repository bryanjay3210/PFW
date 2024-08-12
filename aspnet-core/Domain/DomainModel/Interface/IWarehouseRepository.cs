using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;

namespace Domain.DomainModel.Interface
{
    public interface IWarehouseRepository : IRepository<Warehouse>
    {
        #region Get Data
        Task<List<Warehouse>> GetWarehouses();
        Task<Warehouse?> GetWarehouse(int warehouseId);
        Task<List<WarehousePartDTO>> GetWarehousePartsByProductId(int productId);
        #endregion

        #region Save Data
        Task<List<Warehouse>> Create(Warehouse warehouse);
        Task<List<Warehouse>> Update(Warehouse warehouse);
        Task<List<Warehouse>> Delete(List<int> warehouseIds);
        Task<List<Warehouse>> SoftDelete(List<int> warehouseIds);
        #endregion
    }
}
