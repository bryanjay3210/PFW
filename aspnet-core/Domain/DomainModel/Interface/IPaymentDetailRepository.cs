using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO.Paginated;

namespace Domain.DomainModel.Interface
{
    public interface IPaymentDetailRepository : IRepository<PaymentDetail>
    {
        #region Get Data
        Task<PaymentDetail?> GetPaymentDetail(int paymentDetailId);
        Task<List<PaymentDetail>> GetPaymentDetailsByPaymentId(int paymentId);
        
        #endregion

        #region Save Data
        Task<PaymentDetail> Create(PaymentDetail paymentDetail);
        Task<bool> Update(PaymentDetail paymentDetail);
        Task<List<PaymentDetail>> Delete(List<int> paymentDetailIds);
        Task<List<PaymentDetail>> SoftDelete(List<int> paymentDetailIds);
        #endregion
    }
}
