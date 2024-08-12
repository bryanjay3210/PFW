using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class PurchaseOrderDetailRepository : IPurchaseOrderDetailRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public PurchaseOrderDetailRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<PurchaseOrderDetail>> GetPurchaseOrderDetails(int purchaseOrderId)
        {
            return await _context.PurchaseOrderDetails.ToListAsync();
        }

        public async Task<PurchaseOrderDetail?> GetPurchaseOrderDetail(int purchaseOrderDetailId)
        {
            var result = await _context.PurchaseOrderDetails.FindAsync(purchaseOrderDetailId);
            if (result == null)
                return null;

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<PurchaseOrderDetail>> Create(PurchaseOrderDetail purchaseOrderDetail)
        {
            _context.PurchaseOrderDetails.Add(purchaseOrderDetail);
            await _context.SaveEntitiesAsync();
            return await _context.PurchaseOrderDetails.ToListAsync();
        }

        public async Task<List<PurchaseOrderDetail>> Update(PurchaseOrderDetail purchaseOrderDetail)
        {
            _context.PurchaseOrderDetails.Update(purchaseOrderDetail);
            await _context.SaveEntitiesAsync();
            return await _context.PurchaseOrderDetails.ToListAsync();
        }

        public async Task<List<PurchaseOrderDetail>> Delete(List<int> purchaseOrderDetailIds)
        {
            var purchaseOrderDetails = _context.PurchaseOrderDetails.Where(a => purchaseOrderDetailIds.Contains(a.Id)).ToList();
            _context.PurchaseOrderDetails.RemoveRange(purchaseOrderDetails);
            await _context.SaveEntitiesAsync();
            return await _context.PurchaseOrderDetails.ToListAsync();
        }

        public async Task<List<PurchaseOrderDetail>> SoftDelete(List<int> purchaseOrderDetailIds)
        {
            var purchaseOrderDetails = _context.PurchaseOrderDetails.Where(a => purchaseOrderDetailIds.Contains(a.Id)).ToList();
            purchaseOrderDetails.ForEach(a => { a.IsDeleted = true; });

            _context.PurchaseOrderDetails.UpdateRange(purchaseOrderDetails);
            await _context.SaveEntitiesAsync();
            return await _context.PurchaseOrderDetails.ToListAsync();
        }
        #endregion
    }
}
