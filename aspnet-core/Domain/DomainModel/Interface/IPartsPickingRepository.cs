using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.DTO.Paginated;

namespace Domain.DomainModel.Interface
{
    public interface IPartsPickingRepository : IRepository<PartsPicking>
    {
        #region Get Data
        Task<List<PartsPicking>> GetPartsPickings();
        Task<PartsPicking?> GetPartsPicking(int partsPickingId);
        Task<PaginatedListDTO<PartsPicking>> GetPartsPickingsPaginated(int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search);
        Task<List<StockOrderDetailDTO>> GetStockOrderDetails(int warehouseFilter);
        #endregion

        #region Save Data
        Task<bool> Create(PartsPicking partsPicking);
        Task<bool> Update(PartsPicking partsPicking);
        Task<bool> SoftDeletePartsPickingDetail(PartsPickingDetail partsPickingDetail);
        Task<bool> Delete(PartsPicking partsPicking);
        Task<List<PartsPicking>> SoftDelete(List<int> partsPickingIds);
        #endregion
    }
}
