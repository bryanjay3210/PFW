using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface IPartsManifestDetailRepository : IRepository<PartsManifestDetail>
    {
        #region Get Data
        Task<List<PartsManifestDetail>> GetPartsManifestDetails();
        Task<PartsManifestDetail?> GetPartsManifestDetail(int partsManifestDetailId);
        #endregion

        #region Save Data
        Task<List<PartsManifestDetail>> Create(PartsManifestDetail partsManifestDetail);
        Task<List<PartsManifestDetail>> Update(PartsManifestDetail partsManifestDetail);
        Task<List<PartsManifestDetail>> Delete(List<int> partsManifestDetailIds);
        Task<List<PartsManifestDetail>> SoftDelete(List<int> partsManifestDetailIds);
        #endregion
    }
}
