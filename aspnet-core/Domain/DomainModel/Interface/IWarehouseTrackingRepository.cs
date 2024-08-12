using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface IWarehouseTrackingRepository : IRepository<WarehouseTracking>
    {
        #region Get Data
        Task<List<WarehouseTracking>> GetWarehouseTrackings();
        Task<WarehouseTracking?> GetWarehouseTracking(int warehouseTrackingId);
        #endregion

        #region Save Data
        Task<List<WarehouseTracking>> Create(WarehouseTracking warehouseTracking);
        Task<List<WarehouseTracking>> Update(WarehouseTracking warehouseTracking);
        Task<List<WarehouseTracking>> Delete(List<int> warehouseTrackingIds);
        Task<List<WarehouseTracking>> SoftDelete(List<int> warehouseTrackingIds);
        #endregion
    }
}
