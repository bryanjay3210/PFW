using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.RolesAndAccess;

namespace Domain.DomainModel.Interface.RolesAndAccess
{
    public interface IRoleRepository : IRepository<Role>
    {
        #region Get Data
        Task<List<Role>> GetRoles();
        Task<Role?> GetRole(int roleId);
        #endregion

        #region Save data
        Task<List<Role?>> Create(Role role);
        Task<List<Role>> Update(Role role);
        Task<List<Role>> Delete(List<int> roleIds);
        Task<List<Role>> SoftDelete(List<int> roleIds);
        #endregion
    }
}
