using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.RolesAndAccess;

namespace Domain.DomainModel.Interface.RolesAndAccess
{
    public interface IRoleAccessRepository : IRepository<RoleAccess>
    {
        #region Get Data
        Task<List<RoleAccess>> GetRoleAccesses();
        Task<RoleAccess?> GetRoleAccess(int roleAccessId);
        #endregion

        #region Save data
        Task<List<RoleAccess?>> Create(RoleAccess roleAccess);
        Task<List<RoleAccess>> Update(RoleAccess roleAccess);
        Task<List<RoleAccess>> Delete(List<int> roleAccessIds);
        Task<List<RoleAccess>> SoftDelete(List<int> roleAccessIds);
        #endregion
    }
}
