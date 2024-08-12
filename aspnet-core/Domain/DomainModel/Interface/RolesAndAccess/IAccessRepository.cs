using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.RolesAndAccess;

namespace Domain.DomainModel.Interface.RolesAndAccess
{
    public interface IAccessRepository : IRepository<Access>
    {
        #region Get Data
        Task<List<Access>> GetAccesses();
        Task<Access?> GetAccess(int accessId);
        #endregion

        #region Save data
        Task<List<Access>> Create(Access access);
        Task<List<Access>> Update(Access access);
        Task<List<Access>> Delete(List<int> accessIds);
        Task<List<Access>> SoftDelete(List<int> accessIds);
        #endregion
    }
}
