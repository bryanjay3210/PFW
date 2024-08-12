using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.DTO.Paginated;

namespace Domain.DomainModel.Interface
{
    public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
    {
        #region Get Data
        Task<List<PurchaseOrder>> GetPurchaseOrders();
        Task<PurchaseOrder?> GetPurchaseOrder(int purchaseOrderId);
        Task<PaginatedListDTO<PurchaseOrder>> GetPurchaseOrdersPaginated(int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search);
        Task<PaginatedListDTO<PurchaseOrder>> GetPurchaseOrdersByDatePaginated(int pageSize, int pageIndex, DateTime fromDate, DateTime toDate);

        Task<DailyVendorSalesSummaryDTO> GetDailyVendorSalesSummary(DateTime currentDate);
        Task<DailyVendorSalesSummaryDTO> GetDailyVendorSalesSummaryByDate(DateTime fromDate, DateTime toDate);
        #endregion

        #region Save Data
        Task<bool> Create(PurchaseOrder purchaseOrder);
        Task<bool> Update(PurchaseOrder purchaseOrder);
        Task<bool> SoftDeletePurchaseOrderDetail(PurchaseOrderDetail purchaseOrderDetail);
        Task<bool> Delete(PurchaseOrder purchaseOrder);
        Task<List<PurchaseOrder>> SoftDelete(List<int> purchaseOrderIds);
        #endregion
    }
}
