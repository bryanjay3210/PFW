using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO.Paginated;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class CustomerCreditRepository : ICustomerCreditRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public CustomerCreditRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data

        public async Task<CustomerCredit?> GetCustomerCredit(int customerCreditId)
        {
            var result = await _context.CustomerCredits.FindAsync(customerCreditId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<List<CustomerCredit>> GetCustomerCreditsByCustomerId(int customerId)
        {
            var result = await _context.CustomerCredits.Where(e => e.CustomerId == customerId && e.IsActive == true).ToListAsync();
            return result;
        }

        #endregion

        #region Save Data

        public async Task<CustomerCredit> Create(CustomerCredit customerCredit)
        {
            if (customerCredit.PreviousRecordId > 0)
            {
                var prevCustomerCredit = await _context.CustomerCredits.Where(e => e.Id == customerCredit.PreviousRecordId).FirstOrDefaultAsync();
                if (prevCustomerCredit != null)
                {
                    prevCustomerCredit.IsActive = false;
                    _context.CustomerCredits.Update(customerCredit);
                }
            }

            _context.CustomerCredits.Add(customerCredit);
            await _context.SaveEntitiesAsync();
            return customerCredit;
        }

        public async Task<bool> Update(CustomerCredit customerCredit)
        {
            _context.CustomerCredits.Update(customerCredit);
            return await _context.SaveEntitiesAsync();
        }

        public async Task<List<CustomerCredit>> Delete(List<int> customerCreditIds)
        {
            var customerCredits = _context.CustomerCredits.Where(a => customerCreditIds.Contains(a.Id)).ToList();
            _context.CustomerCredits.RemoveRange(customerCredits);
            await _context.SaveEntitiesAsync();
            return await _context.CustomerCredits.ToListAsync();
        }

        public async Task<List<CustomerCredit>> SoftDelete(List<int> customerCreditIds)
        {
            var customerCredits = _context.CustomerCredits.Where(a => customerCreditIds.Contains(a.Id)).ToList();
            customerCredits.ForEach(a => { a.IsDeleted = true; });

            _context.CustomerCredits.UpdateRange(customerCredits);
            await _context.SaveEntitiesAsync();
            return await _context.CustomerCredits.ToListAsync();
        }

        #endregion
    }
}
