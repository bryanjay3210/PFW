using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface IContactRepository : IRepository<Contact>
    {
        #region Get Data
        Task<List<Contact>> GetContacts();
        Task<Contact?> GetContact(int contactId);
        Task<List<Contact>> GetContactsByCustomerId(int customerId);
        #endregion

        #region Save data
        Task<List<Contact>> Create(Contact contact);
        Task<List<Contact>> Update(Contact contact);
        Task<List<Contact>> Delete(List<int> contactIds);
        Task<List<Contact>> SoftDelete(List<int> contactIds);
        #endregion
    }
}
