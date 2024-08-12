using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface IAutomobileRepository : IRepository<Automobile>
    {
        #region Get Data
        Task<List<Automobile>> GetAutomobiles();
        Task<Automobile?> GetAutomobile(int automobileId);
        #endregion

        #region Save Data
        Task<List<Automobile>> Create(Automobile automobile);
        Task<List<Automobile>> Update(Automobile automobile);
        Task<List<Automobile>> Delete(List<int> automobileIds);
        Task<List<Automobile>> SoftDelete(List<int> automobileIds);
        #endregion
    }
}
