using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface IVendorCatalogRepository : IRepository<VendorCatalog>
    {
        #region Get Data
        Task<List<VendorCatalog>> GetVendorCatalogs();
        Task<VendorCatalog?> GetVendorCatalog(int vendorCatalogId);
        Task<List<VendorCatalog>> GetVendorCatalogsByPartsLinkNumbers(List<string> partsLinkNumbers);
        #endregion

        #region Save Data
        Task<List<VendorCatalog>> Create(VendorCatalog vendorCatalog);
        Task<List<VendorCatalog>> Update(VendorCatalog vendorCatalog);
        Task<List<VendorCatalog>> Delete(List<int> vendorCatalogIds);
        Task<List<VendorCatalog>> SoftDelete(List<int> vendorCatalogIds);

        Task<List<VendorCatalog>> CreateByProduct(VendorCatalog vendorCatalog, List<string> partsLinkNumbers);
        Task<List<VendorCatalog>> UpdateByProduct(VendorCatalog vendorCatalog, List<string> partsLinkNumbers);
        #endregion
    }
}
