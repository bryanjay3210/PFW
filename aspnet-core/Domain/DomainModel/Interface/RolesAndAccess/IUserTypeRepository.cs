using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.RolesAndAccess;

namespace Domain.DomainModel.Interface.RolesAndAccess
{
    public interface IUserTypeRepository : IRepository<UserType>
    {
        #region Get Data
        Task<List<UserType>> GetUserTypes();
        Task<UserType?> GetUserType(int userTypeId);
        #endregion

        #region Save data
        Task<List<UserType?>> Create(UserType userType);
        Task<List<UserType>> Update(UserType userType);
        Task<List<UserType>> Delete(List<int> userTypeIds);
        Task<List<UserType>> SoftDelete(List<int> userTypeIds);
        #endregion
    }
}
