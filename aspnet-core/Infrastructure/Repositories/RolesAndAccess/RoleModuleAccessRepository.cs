using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.RolesAndAccess;
using Domain.DomainModel.Interface;
using Domain.DomainModel.Interface.RolesAndAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class RoleModuleAccessRepository : IRoleModuleAccessRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public RoleModuleAccessRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<RoleModuleAccess>> GetRoleModuleAccesses()
        {
            return await _context.RoleModuleAccesses.ToListAsync();
        }

        public async Task<RoleModuleAccess?> GetRoleModuleAccess(int roleModuleAccessId)
        {
            var result = await _context.RoleModuleAccesses.FindAsync(roleModuleAccessId);
            if (result == null)
                return null;

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<RoleModuleAccess>> Create(RoleModuleAccess roleModuleAccess)
        {
            _context.RoleModuleAccesses.Add(roleModuleAccess);
            await _context.SaveEntitiesAsync();
            return await _context.RoleModuleAccesses.ToListAsync();
        }

        public async Task<List<RoleModuleAccess>> Update(RoleModuleAccess roleModuleAccess)
        {
            _context.RoleModuleAccesses.Update(roleModuleAccess);
            await _context.SaveEntitiesAsync();
            return await _context.RoleModuleAccesses.ToListAsync();
        }

        public async Task<List<RoleModuleAccess>> Delete(List<int> roleModuleAccessIds)
        {
            var roleModuleAccesses = _context.RoleModuleAccesses.Where(a => roleModuleAccessIds.Contains(a.Id)).ToList();
            _context.RoleModuleAccesses.RemoveRange(roleModuleAccesses);
            await _context.SaveEntitiesAsync();
            return await _context.RoleModuleAccesses.ToListAsync();
        }

        public async Task<List<RoleModuleAccess>> SoftDelete(List<int> roleModuleAccessIds)
        {
            var roleModuleAccesses = _context.RoleModuleAccesses.Where(a => roleModuleAccessIds.Contains(a.Id)).ToList();
            roleModuleAccesses.ForEach(a => { a.IsDeleted = true; });

            _context.RoleModuleAccesses.UpdateRange(roleModuleAccesses);
            await _context.SaveEntitiesAsync();
            return await _context.RoleModuleAccesses.ToListAsync();
        }
        #endregion
    }
}
