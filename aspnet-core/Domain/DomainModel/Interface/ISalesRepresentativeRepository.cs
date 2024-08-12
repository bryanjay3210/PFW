using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface ISalesRepresentativeRepository : IRepository<SalesRepresentative>
    {
        #region Get Data
        Task<List<SalesRepresentative>> GetSalesRepresentatives();
        Task<SalesRepresentative?> GetSalesRepresentative(int salesRepresentativeId);
        #endregion

        #region Save Data
        Task<List<SalesRepresentative>> Create(SalesRepresentative salesRepresentative);
        Task<List<SalesRepresentative>> Update(SalesRepresentative salesRepresentative);
        Task<List<SalesRepresentative>> Delete(List<int> salesRepresentativeIds);
        Task<List<SalesRepresentative>> SoftDelete(List<int> salesRepresentativeIds);
        #endregion
    }
}
