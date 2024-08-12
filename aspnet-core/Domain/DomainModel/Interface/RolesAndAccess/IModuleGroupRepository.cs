using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.RolesAndAccess;

namespace Domain.DomainModel.Interface.RolesAndAccess
{
    public interface IModuleGroupRepository : IRepository<ModuleGroup>
    {
        #region Get Data
        Task<List<ModuleGroup>> GetModuleGroups();
        Task<ModuleGroup?> GetModuleGroup(int moduleGroupId);
        #endregion

        #region Save data
        Task<List<ModuleGroup?>> Create(ModuleGroup moduleGroup);
        Task<List<ModuleGroup>> Update(ModuleGroup moduleGroup);
        Task<List<ModuleGroup>> Delete(List<int> moduleGroupIds);
        Task<List<ModuleGroup>> SoftDelete(List<int> moduleGroupIds);
        #endregion
    }
}
