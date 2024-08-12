using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO.Paginated;

namespace Domain.DomainModel.Interface
{
    public interface ICustomerCreditRepository : IRepository<CustomerCredit>
    {
        #region Get Data
        Task<CustomerCredit?> GetCustomerCredit(int customerCreditId);
        Task<List<CustomerCredit>> GetCustomerCreditsByCustomerId(int customerId);
        #endregion

        #region Save Data
        Task<CustomerCredit> Create(CustomerCredit customerCredit);
        Task<bool> Update(CustomerCredit customerCredit);
        Task<List<CustomerCredit>> Delete(List<int> customerCreditIds);
        Task<List<CustomerCredit>> SoftDelete(List<int> customerCreditIds);
        #endregion
    }
}
