using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface IPurchaseOrderDetailRepository : IRepository<PurchaseOrderDetail>
    {
        #region Get Data
        Task<List<PurchaseOrderDetail>> GetPurchaseOrderDetails(int purchaseOrderId);
        Task<PurchaseOrderDetail?> GetPurchaseOrderDetail(int purchaseOrderDetailId);
        #endregion

        #region Save Data
        Task<List<PurchaseOrderDetail>> Create(PurchaseOrderDetail purchaseOrderDetail);
        Task<List<PurchaseOrderDetail>> Update(PurchaseOrderDetail purchaseOrderDetail);
        Task<List<PurchaseOrderDetail>> Delete(List<int> purchaseOrderDetailIds);
        Task<List<PurchaseOrderDetail>> SoftDelete(List<int> purchaseOrderDetailIds);
        #endregion
    }
}
