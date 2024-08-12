using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface IOrderNoteRepository : IRepository<OrderNote>
    {
        #region Get Data
        Task<List<OrderNote>> GetOrderNotes();
        Task<List<OrderNote>> GetOrderNotesByOrderId(int orderId);
        #endregion

        #region Save Data
        Task<List<OrderNote>> Create(OrderNote orderNote);
        Task<List<OrderNote>> Update(OrderNote orderNote);
        Task<List<OrderNote>> Delete(List<int> orderNoteIds);
        Task<List<OrderNote>> SoftDelete(List<int> orderNoteIds);
        #endregion
    }
}
