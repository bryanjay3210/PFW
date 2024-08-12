
using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface.User
{
    public interface IUserRepository : IRepository<Entity.User>
    {
        #region Get Data
        Task<List<Entity.User>> GetUsers();
        Task<Entity.User?> GetUserById(int userId);
        Task<Entity.User?> GetUserByName(string userName);
        Task<Entity.User?> GetUserByEmail(string email);
        Task<UserNotificationDTO> GetUserNotificationsByUserId(int userId, string creditMemoFilterDate);
        Task<Entity.User?> Login(UserDTO user);
        #endregion

        #region Save data
        Task<Entity.User?> Create(Entity.User user);
        Task<List<Entity.User>> Update(Entity.User user);
        Task<List<Entity.User>> Delete(List<int> userIds);
        Task<List<Entity.User>> SoftDelete(List<int> userIds);
        Task<Entity.User?> Register(Entity.User user);
        #endregion
    }
}
