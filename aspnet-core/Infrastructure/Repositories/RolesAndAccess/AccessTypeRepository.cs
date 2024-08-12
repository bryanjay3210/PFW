using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.RolesAndAccess;
using Domain.DomainModel.Interface;
using Domain.DomainModel.Interface.RolesAndAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class AccessTypeRepository : IAccessTypeRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public AccessTypeRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<AccessType>> GetAccessTypes()
        {
            return await _context.AccessTypes.ToListAsync();
        }

        public async Task<AccessType?> GetAccessType(int accessTypeId)
        {
            var result = await _context.AccessTypes.FindAsync(accessTypeId);
            if (result == null)
                return null;

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<AccessType>> Create(AccessType accessType)
        {
            _context.AccessTypes.Add(accessType);
            await _context.SaveEntitiesAsync();
            return await _context.AccessTypes.ToListAsync();
        }

        public async Task<List<AccessType>> Update(AccessType accessType)
        {
            _context.AccessTypes.Update(accessType);
            await _context.SaveEntitiesAsync();
            return await _context.AccessTypes.ToListAsync();
        }

        public async Task<List<AccessType>> Delete(List<int> accessTypeIds)
        {
            var accessTypes = _context.AccessTypes.Where(a => accessTypeIds.Contains(a.Id)).ToList();
            _context.AccessTypes.RemoveRange(accessTypes);
            await _context.SaveEntitiesAsync();
            return await _context.AccessTypes.ToListAsync();
        }

        public async Task<List<AccessType>> SoftDelete(List<int> accessTypeIds)
        {
            var accessTypes = _context.AccessTypes.Where(a => accessTypeIds.Contains(a.Id)).ToList();
            accessTypes.ForEach(a => { a.IsDeleted = true; });

            _context.AccessTypes.UpdateRange(accessTypes);
            await _context.SaveEntitiesAsync();
            return await _context.AccessTypes.ToListAsync();
        }
        #endregion
    }
}
