using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.DTO.Paginated;

namespace Domain.DomainModel.Interface
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        #region Get Data
        Task<PaginatedListDTO<Payment>> GetPaymentsPaginated(int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search);
        Task<PaginatedListDTO<Payment>> GetPaymentsByDatePaginated(int pageSize, int pageIndex, DateTime fromDate, DateTime toDate);
        Task<Payment?> GetPayment(int paymentId);
        Task<List<PaymentHistoryDTO>> GetPaymentHistoryByOrderNumber(int orderNumber);
        Task<DailyPaymentSummaryDTO> GetDailyPaymentSummary(DateTime currentDate);
        Task<DailyPaymentSummaryDTO> GetPaymentSummaryByDate(DateTime fromDate, DateTime toDate);
        #endregion

        #region Save Data
        Task<Payment> Create(Payment payment);
        Task<Payment> CreateRefund(Payment payment);
        Task<bool> Update(Payment payment);
        Task<List<Payment>> Delete(List<int> paymentIds);
        Task<List<Payment>> SoftDelete(List<int> paymentIds);
        #endregion
    }
}
