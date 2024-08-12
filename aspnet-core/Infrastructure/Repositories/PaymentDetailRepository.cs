using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO.Paginated;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class PaymentDetailRepository : IPaymentDetailRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public PaymentDetailRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data

        public async Task<PaymentDetail?> GetPaymentDetail(int paymentDetailId)
        {
            var result = await _context.PaymentDetails.FindAsync(paymentDetailId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<List<PaymentDetail>> GetPaymentDetailsByPaymentId(int paymentId)
        {
            var result = await _context.PaymentDetails.Where(e => e.PaymentId == paymentId).ToListAsync();
            return result;
        }

        #endregion

        #region Save Data

        public async Task<PaymentDetail> Create(PaymentDetail paymentDetail)
        {
            _context.PaymentDetails.Add(paymentDetail);
            await _context.SaveEntitiesAsync();
            return paymentDetail;
        }

        public async Task<bool> Update(PaymentDetail paymentDetail)
        {
            _context.PaymentDetails.Update(paymentDetail);
            return await _context.SaveEntitiesAsync();
        }

        public async Task<List<PaymentDetail>> Delete(List<int> paymentDetailIds)
        {
            var paymentDetails = _context.PaymentDetails.Where(a => paymentDetailIds.Contains(a.Id)).ToList();
            _context.PaymentDetails.RemoveRange(paymentDetails);
            await _context.SaveEntitiesAsync();
            return await _context.PaymentDetails.ToListAsync();
        }

        public async Task<List<PaymentDetail>> SoftDelete(List<int> paymentDetailIds)
        {
            var paymentDetails = _context.PaymentDetails.Where(a => paymentDetailIds.Contains(a.Id)).ToList();
            paymentDetails.ForEach(a => { a.IsDeleted = true; });

            _context.PaymentDetails.UpdateRange(paymentDetails);
            await _context.SaveEntitiesAsync();
            return await _context.PaymentDetails.ToListAsync();
        }

        #endregion
    }
}
