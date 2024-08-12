using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public ContactRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<Contact>> GetContacts()
        {
            return await _context.Contacts.ToListAsync();
        }

        public async Task<Contact?> GetContact(int contactId)
        {
            var result = await _context.Contacts.FindAsync(contactId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<List<Contact>> GetContactsByCustomerId(int customerId)
        {
            return await _context.Contacts.Where(e => e.CustomerId == customerId && e.IsDeleted == false).ToListAsync();
        }
        #endregion

        #region Save Data
        public async Task<List<Contact>> Create(Contact contact)
        {
            if (contact.LocationId == null)
            {
                var location = await _context.Locations.FirstOrDefaultAsync(e => e.CustomerId == contact.CustomerId);
                if (location != null)
                {
                    contact.LocationId = location.Id;
                }
            }

            if (contact.PositionTypeId == null)
            {
                var positionType = await _context.PositionTypes.FirstOrDefaultAsync();
                if (positionType != null)
                {
                    contact.PositionTypeId = positionType.Id;
                }
            }

            _context.Contacts.Add(contact);
            await _context.SaveEntitiesAsync();
            return await _context.Contacts.Where(c => c.CustomerId == contact.CustomerId).ToListAsync();
        }

        public async Task<List<Contact>> Update(Contact contact)
        {
            _context.Contacts.Update(contact);
            await _context.SaveEntitiesAsync();
            return await _context.Contacts.Where(c => c.CustomerId == contact.CustomerId).ToListAsync();
        }

        public async Task<List<Contact>> Delete(List<int> contactIds)
        {
            var contacts = _context.Contacts.Where(a => contactIds.Contains(a.Id)).ToList();
            _context.Contacts.RemoveRange(contacts);
            await _context.SaveEntitiesAsync();
            return await _context.Contacts.Where(c => c.CustomerId == contacts[0].CustomerId).ToListAsync();
        }

        public async Task<List<Contact>> SoftDelete(List<int> contactIds)
        {
            var contacts = _context.Contacts.Where(a => contactIds.Contains(a.Id)).ToList();
            contacts.ForEach(c => { c.IsDeleted = true; });

            _context.Contacts.UpdateRange(contacts);
            await _context.SaveEntitiesAsync();
            return await _context.Contacts.ToListAsync();
        }
        #endregion
    }
}
