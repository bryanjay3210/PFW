using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class PaymentTermRepository : IPaymentTermRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public PaymentTermRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<PaymentTerm?> GetPaymentTerm(int paymentTermId)
        {
            var result = await _context.PaymentTerms.FindAsync(paymentTermId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<List<PaymentTerm>> GetPaymentTerms()
        {
            return await _context.PaymentTerms.ToListAsync();
        }
        #endregion

        #region Save Data
        public async Task<List<PaymentTerm>> Create(PaymentTerm paymentTerm)
        {
            _context.PaymentTerms.Add(paymentTerm);
            await _context.SaveEntitiesAsync();
            return await _context.PaymentTerms.ToListAsync();
        }

        public async Task<List<PaymentTerm>> Update(PaymentTerm paymentTerm)
        {
            _context.PaymentTerms.Update(paymentTerm);
            await _context.SaveEntitiesAsync();
            return await _context.PaymentTerms.ToListAsync();
        }

        public async Task<List<PaymentTerm>> Delete(List<int> paymentTermIds)
        {
            var paymentTerms = _context.PaymentTerms.Where(a => paymentTermIds.Contains(a.Id)).ToList();
            _context.PaymentTerms.RemoveRange(paymentTerms);
            await _context.SaveEntitiesAsync();
            return await _context.PaymentTerms.ToListAsync();
        }

        public async Task<List<PaymentTerm>> SoftDelete(List<int> paymentTermIds)
        {
            var paymentTerms = _context.PaymentTerms.Where(a => paymentTermIds.Contains(a.Id)).ToList();
            paymentTerms.ForEach(c => { c.IsDeleted = true; });

            _context.PaymentTerms.UpdateRange(paymentTerms);
            await _context.SaveEntitiesAsync();
            return await _context.PaymentTerms.ToListAsync();
        }
        #endregion
    }
}
