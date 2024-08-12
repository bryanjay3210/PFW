using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.DTO.Paginated;

namespace Domain.DomainModel.Interface
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        #region Get Data
        Task<List<Customer>> GetCustomers();
        Task<PaginatedListDTO<Customer>> GetCustomersPaginated(int pageSize, int pageIndex, string? sortColumn = "CustomerName", string? sortOrder = "ASC", string? search = "");

        Task<List<CustomerDTO>> GetCustomersList();
        Task<PaginatedListDTO<CustomerDTO>> GetCustomersListPaginated(int pageSize, int pageIndex, string? sortColumn = "CustomerName", string? sortOrder = "ASC", string? search = "");
        Task<PaginatedListDTO<CustomerDTO>> GetReportCustomersListPaginated(int pageSize, int pageIndex, string? sortColumn = "CustomerName", string? sortOrder = "ASC", string? search = "", int? searchPaymentTermId = 0, string? searchState = "");

        Task<Customer?> GetCustomer(int customerId);
        Task<CustomerDTO?> GetCustomerById(int customerId);
        Task<List<CustomerEmailDTO>?> GetCustomerEmailsById(int customerId);
        Task<CustomerDTO?> GetCustomerByAccountNumber(int accountNumber);
        #endregion

        #region Save Data
        Task<Customer> Create(Customer customer);
        Task<bool> Update(Customer customer);
        Task<List<Customer>> Delete(List<int> customerIds);
        Task<List<Customer>> SoftDelete(List<int> customerIds);
        #endregion
    }
}
