using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.RolesAndAccess;

namespace Domain.DomainModel.Interface.RolesAndAccess
{
    public interface IRoleModuleAccessRepository : IRepository<RoleModuleAccess>
    {
        #region Get Data
        Task<List<RoleModuleAccess>> GetRoleModuleAccesses();
        Task<RoleModuleAccess?> GetRoleModuleAccess(int roleModuleAccessId);
        #endregion

        #region Save data
        Task<List<RoleModuleAccess?>> Create(RoleModuleAccess roleModuleAccess);
        Task<List<RoleModuleAccess>> Update(RoleModuleAccess roleModuleAccess);
        Task<List<RoleModuleAccess>> Delete(List<int> roleModuleAccessIds);
        Task<List<RoleModuleAccess>> SoftDelete(List<int> roleModuleAccessIds);
        #endregion
    }
}
