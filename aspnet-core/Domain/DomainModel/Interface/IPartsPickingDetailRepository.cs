using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface IPartsPickingDetailRepository : IRepository<PartsPickingDetail>
    {
        #region Get Data
        Task<List<PartsPickingDetail>> GetPartsPickingDetails(int partsPickingId);
        Task<PartsPickingDetail?> GetPartsPickingDetail(int partsPickingDetailId);
        #endregion

        #region Save Data
        Task<List<PartsPickingDetail>> Create(PartsPickingDetail partsPickingDetail);
        Task<List<PartsPickingDetail>> Update(PartsPickingDetail partsPickingDetail);
        Task<List<PartsPickingDetail>> Delete(List<int> partsPickingDetailIds);
        Task<List<PartsPickingDetail>> SoftDelete(List<int> partsPickingDetailIds);
        #endregion
    }
}
