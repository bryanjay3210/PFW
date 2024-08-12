using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface IPaymentTermRepository : IRepository<PaymentTerm>
    {
        #region Get Data
        Task<List<PaymentTerm>> GetPaymentTerms();
        Task<PaymentTerm?> GetPaymentTerm(int paymentTermId);
        #endregion

        #region Save Data
        Task<List<PaymentTerm>> Create(PaymentTerm paymentTerm);
        Task<List<PaymentTerm>> Update(PaymentTerm paymentTerm);
        Task<List<PaymentTerm>> Delete(List<int> paymentTermIds);
        Task<List<PaymentTerm>> SoftDelete(List<int> paymentTermIds);
        #endregion
    }
}
