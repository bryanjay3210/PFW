using Domain.DomainModel.Entity.RolesAndAccess;

namespace Domain.DomainModel.Interface.RolesAndAccess
{
    public interface IAccessTypeRepository : IRepository<AccessType>
    {
        #region Get Data
        Task<List<AccessType>> GetAccessTypes();
        Task<AccessType?> GetAccessType(int accessTypeId);
        #endregion

        #region Save data
        Task<List<AccessType?>> Create(AccessType accessType);
        Task<List<AccessType>> Update(AccessType accessType);
        Task<List<AccessType>> Delete(List<int> accessTypeIds);
        Task<List<AccessType>> SoftDelete(List<int> accessTypeIds);
        #endregion
    }
}
