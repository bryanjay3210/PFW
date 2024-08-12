using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface ICustomerNoteRepository : IRepository<CustomerNote>
    {
        #region Get Data
        Task<List<CustomerNote>> GetCustomerNotes();
        Task<CustomerNote?> GetCustomerNote(int customerNoteId);
        #endregion

        #region Save Data
        Task<List<CustomerNote>> Create(CustomerNote customerNote);
        Task<List<CustomerNote>> Update(CustomerNote customerNote);
        Task<List<CustomerNote>> Delete(List<int> customerNoteIds);
        Task<List<CustomerNote>> SoftDelete(List<int> customerNoteIds);
        Task<List<CustomerNote>> GetCustomerNotesByCustomerId(int customerId);
        #endregion
    }
}
