using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.RolesAndAccess;
using Domain.DomainModel.Interface;
using Domain.DomainModel.Interface.RolesAndAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class ModuleGroupRepository : IModuleGroupRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public ModuleGroupRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<ModuleGroup>> GetModuleGroups()
        {
            var moduleGroups = await _context.ModuleGroups.ToListAsync();
            
            foreach(var mg in moduleGroups)
            {
                mg.Modules = await _context.Modules.Where(m => m.ModuleGroupId == mg.Id && m.IsActive == true).ToListAsync();
            }

            return moduleGroups.OrderBy(e => e.Name).ToList();
        }

        public async Task<ModuleGroup?> GetModuleGroup(int moduleGroupId)
        {
            var result = await _context.ModuleGroups.FindAsync(moduleGroupId);
            if (result == null)
                return null;

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<ModuleGroup>> Create(ModuleGroup moduleGroup)
        {
            _context.ModuleGroups.Add(moduleGroup);
            await _context.SaveEntitiesAsync();
            return await _context.ModuleGroups.OrderBy(e => e.Name).ToListAsync();
        }

        public async Task<List<ModuleGroup>> Update(ModuleGroup moduleGroup)
        {
            _context.ModuleGroups.Update(moduleGroup);
            await _context.SaveEntitiesAsync();
            return await _context.ModuleGroups.OrderBy(e => e.Name).ToListAsync();
        }

        public async Task<List<ModuleGroup>> Delete(List<int> moduleGroupIds)
        {
            var moduleGroups = _context.ModuleGroups.Where(a => moduleGroupIds.Contains(a.Id)).ToList();
            _context.ModuleGroups.RemoveRange(moduleGroups);
            await _context.SaveEntitiesAsync();
            return await _context.ModuleGroups.OrderBy(e => e.Name).ToListAsync();
        }

        public async Task<List<ModuleGroup>> SoftDelete(List<int> moduleGroupIds)
        {
            var moduleGroups = _context.ModuleGroups.Where(a => moduleGroupIds.Contains(a.Id)).ToList();
            moduleGroups.ForEach(a => { a.IsDeleted = true; });

            _context.ModuleGroups.UpdateRange(moduleGroups);
            await _context.SaveEntitiesAsync();
            return await _context.ModuleGroups.OrderBy(e => e.Name).ToListAsync();
        }
        #endregion
    }
}
