using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.RolesAndAccess;
using Domain.DomainModel.Interface;
using Domain.DomainModel.Interface.RolesAndAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class RoleAccessRepository : IRoleAccessRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public RoleAccessRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<RoleAccess>> GetRoleAccesses()
        {
            return await _context.RoleAccesses.ToListAsync();
        }

        public async Task<RoleAccess?> GetRoleAccess(int roleAccessId)
        {
            var result = await _context.RoleAccesses.FindAsync(roleAccessId);
            if (result == null)
                return null;

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<RoleAccess>> Create(RoleAccess roleAccess)
        {
            _context.RoleAccesses.Add(roleAccess);
            await _context.SaveEntitiesAsync();
            return await _context.RoleAccesses.ToListAsync();
        }

        public async Task<List<RoleAccess>> Update(RoleAccess roleAccess)
        {
            _context.RoleAccesses.Update(roleAccess);
            await _context.SaveEntitiesAsync();
            return await _context.RoleAccesses.ToListAsync();
        }

        public async Task<List<RoleAccess>> Delete(List<int> roleAccessIds)
        {
            var roleAccesses = _context.RoleAccesses.Where(a => roleAccessIds.Contains(a.Id)).ToList();
            _context.RoleAccesses.RemoveRange(roleAccesses);
            await _context.SaveEntitiesAsync();
            return await _context.RoleAccesses.ToListAsync();
        }

        public async Task<List<RoleAccess>> SoftDelete(List<int> roleAccessIds)
        {
            var roleAccesses = _context.RoleAccesses.Where(a => roleAccessIds.Contains(a.Id)).ToList();
            roleAccesses.ForEach(a => { a.IsDeleted = true; });

            _context.RoleAccesses.UpdateRange(roleAccesses);
            await _context.SaveEntitiesAsync();
            return await _context.RoleAccesses.ToListAsync();
        }
        #endregion
    }
}
