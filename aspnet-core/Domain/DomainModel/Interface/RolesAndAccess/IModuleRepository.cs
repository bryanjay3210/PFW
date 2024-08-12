using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.RolesAndAccess;

namespace Domain.DomainModel.Interface.RolesAndAccess
{
    public interface IModuleRepository : IRepository<Module>
    {
        #region Get Data
        Task<List<Module>> GetModules();
        Task<Module?> GetModule(int moduleId);
        Task<List<Module>> GetModulesByModuleGroup(int moduleGroupId);
        #endregion

        #region Save data
        Task<List<Module?>> Create(Module module);
        Task<List<Module>> Update(Module module);
        Task<List<Module>> Delete(List<int> moduleIds);
        Task<List<Module>> SoftDelete(List<int> moduleIds);
        #endregion
    }
}
