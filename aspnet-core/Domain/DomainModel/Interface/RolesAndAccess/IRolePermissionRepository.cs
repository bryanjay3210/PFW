using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.RolesAndAccess;

namespace Domain.DomainModel.Interface.RolesAndAccess
{
    public interface IRolePermissionRepository : IRepository<RolePermission>
    {
        #region Get Data
        Task<List<RolePermission>> GetRolePermissions();
        Task<RolePermission?> GetRolePermission(int rolePermissionId);
        #endregion

        #region Save data
        Task<List<RolePermission?>> Create(RolePermission rolePermission);
        Task<List<RolePermission>> Update(RolePermission rolePermission);
        Task<List<RolePermission>> Delete(List<int> rolePermissionIds);
        Task<List<RolePermission>> SoftDelete(List<int> rolePermissionIds);
        #endregion
    }
}
