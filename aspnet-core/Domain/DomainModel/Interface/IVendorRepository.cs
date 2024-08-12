using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;

namespace Domain.DomainModel.Interface
{
    public interface IVendorRepository : IRepository<Vendor>
    {
        #region Get Data
        Task<List<Vendor>> GetVendors();
        Task<List<Vendor>> GetVendorsByState(string state);
        Task<Vendor?> GetVendor(int vendorId);
        Task<List<VendorOrderDTO>> GetVendorOrdersByVendorCode(string vendorCode);
        #endregion

        #region Save Data
        Task<List<Vendor>> Create(Vendor vendor);
        Task<List<Vendor>> Update(Vendor vendor);
        Task<List<Vendor>> Delete(List<int> vendorIds);
        Task<List<Vendor>> SoftDelete(List<int> vendorIds);
        #endregion
    }
}
