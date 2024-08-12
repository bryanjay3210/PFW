using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.RolesAndAccess;
using Domain.DomainModel.Interface;
using Domain.DomainModel.Interface.RolesAndAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class UserTypeRepository : IUserTypeRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public UserTypeRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<UserType>> GetUserTypes()
        {
            return await _context.UserTypes.ToListAsync();
        }

        public async Task<UserType?> GetUserType(int userTypeId)
        {
            var result = await _context.UserTypes.FindAsync(userTypeId);
            if (result == null)
                return null;

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<UserType>> Create(UserType userType)
        {
            _context.UserTypes.Add(userType);
            await _context.SaveEntitiesAsync();
            return await _context.UserTypes.ToListAsync();
        }

        public async Task<List<UserType>> Update(UserType userType)
        {
            _context.UserTypes.Update(userType);
            await _context.SaveEntitiesAsync();
            return await _context.UserTypes.ToListAsync();
        }

        public async Task<List<UserType>> Delete(List<int> userTypeIds)
        {
            var userTypes = _context.UserTypes.Where(a => userTypeIds.Contains(a.Id)).ToList();
            _context.UserTypes.RemoveRange(userTypes);
            await _context.SaveEntitiesAsync();
            return await _context.UserTypes.ToListAsync();
        }

        public async Task<List<UserType>> SoftDelete(List<int> userTypeIds)
        {
            var userTypes = _context.UserTypes.Where(a => userTypeIds.Contains(a.Id)).ToList();
            userTypes.ForEach(a => { a.IsDeleted = true; });

            _context.UserTypes.UpdateRange(userTypes);
            await _context.SaveEntitiesAsync();
            return await _context.UserTypes.ToListAsync();
        }
        #endregion
    }
}
