using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO.Paginated;

namespace Domain.DomainModel.Interface
{
    public interface IPartsManifestRepository : IRepository<PartsManifest>
    {
        #region Get Data
        Task<List<PartsManifest>> GetPartsManifests();
        Task<PartsManifest?> GetPartsManifest(int partsManifestId);

        Task<PaginatedListDTO<PartsManifest>> GetPartsManifestsPaginated(int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search);
        Task<PaginatedListDTO<PartsManifest>> GetPartsManifestsByDatePaginated(int pageSize, int pageIndex, DateTime fromDate, DateTime toDate);
        #endregion

        #region Save Data
        Task<bool> Create(PartsManifest partsManifest);
        Task<List<PartsManifest>> Update(PartsManifest partsManifest);
        Task<List<PartsManifest>> Delete(List<int> partsManifestIds);
        Task<List<PartsManifest>> SoftDelete(List<int> partsManifestIds);
        #endregion
    }
}
