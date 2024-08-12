using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface IPartsCatalogRepository : IRepository<PartsCatalog>
    {
        #region Get Data
        Task<List<PartsCatalog>> GetPartsCatalogs();
        Task<List<PartsCatalog>> GetPartsCatalogsByProductId(int productId);
        Task<PartsCatalog?> GetPartsCatalog(int partsCatalogId);
        #endregion

        #region Save Data
        Task<List<PartsCatalog>> Create(PartsCatalog partsCatalog);
        Task<List<PartsCatalog>> Update(PartsCatalog partsCatalog);
        Task<List<PartsCatalog>> Delete(List<int> partsCatalogIds);
        Task<List<PartsCatalog>> SoftDelete(List<int> partsCatalogIds);
        #endregion
    }
}
