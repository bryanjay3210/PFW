using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Infrastucture.Repositories
{
    public class CustomerNoteRepository : ICustomerNoteRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public CustomerNoteRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<CustomerNote>> GetCustomerNotes()
        {
            return await _context.CustomerNotes.ToListAsync();
        }

        public async Task<CustomerNote?> GetCustomerNote(int customerNoteId)
        {
            var result = await _context.CustomerNotes.FindAsync(customerNoteId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<List<CustomerNote>> GetCustomerNotesByCustomerId(int customerId)
        {
            var result = new List<CustomerNote>();
            result = await _context.CustomerNotes.Where(e => e.CustomerId == customerId).OrderByDescending(e => e.CreatedDate).ToListAsync();
            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<CustomerNote>> Create(CustomerNote customerNote)
        {
            _context.CustomerNotes.Add(customerNote);
            await _context.SaveEntitiesAsync();
            return await _context.CustomerNotes.Where(e => e.CustomerId == customerNote.CustomerId).OrderByDescending(e => e.CreatedDate).ToListAsync();
        }

        public async Task<List<CustomerNote>> Update(CustomerNote customerNote)
        {
            _context.CustomerNotes.Update(customerNote);
            await _context.SaveEntitiesAsync();
            return await _context.CustomerNotes.Where(e => e.CustomerId == customerNote.CustomerId).ToListAsync();
        }

        public async Task<List<CustomerNote>> Delete(List<int> customerNoteIds)
        {
            var customerNotes = _context.CustomerNotes.Where(a => customerNoteIds.Contains(a.Id)).ToList();
            _context.CustomerNotes.RemoveRange(customerNotes);
            await _context.SaveEntitiesAsync();
            return await _context.CustomerNotes.ToListAsync();
        }

        public async Task<List<CustomerNote>> SoftDelete(List<int> customerNoteIds)
        {
            var customerNotes = _context.CustomerNotes.Where(a => customerNoteIds.Contains(a.Id)).ToList();
            customerNotes.ForEach(cn => { cn.IsDeleted = true; });

            _context.CustomerNotes.UpdateRange(customerNotes);
            await _context.SaveEntitiesAsync();
            return await _context.CustomerNotes.ToListAsync();
        }
        #endregion
    }
}
