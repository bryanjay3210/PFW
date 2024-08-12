using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.RolesAndAccess;
using Domain.DomainModel.Interface;
using Domain.DomainModel.Interface.RolesAndAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class AccessRepository : IAccessRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public AccessRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<Access>> GetAccesses()
        {
            return await _context.Accesses.ToListAsync();
        }

        public async Task<Access?> GetAccess(int accessId)
        {
            var result = await _context.Accesses.FindAsync(accessId);
            if (result == null)
                return null;

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<Access>> Create(Access access)
        {
            _context.Accesses.Add(access);
            await _context.SaveEntitiesAsync();
            return await _context.Accesses.ToListAsync();
        }

        public async Task<List<Access>> Update(Access access)
        {
            _context.Accesses.Update(access);
            await _context.SaveEntitiesAsync();
            return await _context.Accesses.ToListAsync();
        }

        public async Task<List<Access>> Delete(List<int> accessIds)
        {
            var accesss = _context.Accesses.Where(a => accessIds.Contains(a.Id)).ToList();
            _context.Accesses.RemoveRange(accesss);
            await _context.SaveEntitiesAsync();
            return await _context.Accesses.ToListAsync();
        }

        public async Task<List<Access>> SoftDelete(List<int> accessIds)
        {
            var accesss = _context.Accesses.Where(a => accessIds.Contains(a.Id)).ToList();
            accesss.ForEach(a => { a.IsDeleted = true; });

            _context.Accesses.UpdateRange(accesss);
            await _context.SaveEntitiesAsync();
            return await _context.Accesses.ToListAsync();
        }
        #endregion
    }
}
