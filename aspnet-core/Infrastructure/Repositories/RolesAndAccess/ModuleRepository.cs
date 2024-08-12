using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.RolesAndAccess;
using Domain.DomainModel.Interface;
using Domain.DomainModel.Interface.RolesAndAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public ModuleRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<Module>> GetModules()
        {
            return await _context.Modules.ToListAsync();
        }

        public async Task<Module?> GetModule(int moduleId)
        {
            var result = await _context.Modules.FindAsync(moduleId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<List<Module>> GetModulesByModuleGroup(int moduleGroupId)
        {
            var result = await _context.Modules.Where(m => m.ModuleGroupId == moduleGroupId).ToListAsync();
            
            //if (result == null) return null;

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<Module>> Create(Module module)
        {
            _context.Modules.Add(module);
            await _context.SaveEntitiesAsync();
            return await _context.Modules.Where(m => m.ModuleGroupId == module.ModuleGroupId).ToListAsync();
        }

        public async Task<List<Module>> Update(Module module)
        {
            _context.Modules.Update(module);
            await _context.SaveEntitiesAsync();
            return await _context.Modules.Where(m => m.ModuleGroupId == module.ModuleGroupId).ToListAsync();
        }

        public async Task<List<Module>> Delete(List<int> moduleIds)
        {
            var modules = _context.Modules.Where(a => moduleIds.Contains(a.Id)).ToList();
            _context.Modules.RemoveRange(modules);
            await _context.SaveEntitiesAsync();
            return await _context.Modules.Where(m => m.ModuleGroupId == modules[0].ModuleGroupId).ToListAsync();
        }

        public async Task<List<Module>> SoftDelete(List<int> moduleIds)
        {
            var modules = _context.Modules.Where(a => moduleIds.Contains(a.Id)).ToList();
            modules.ForEach(a => { a.IsDeleted = true; });

            _context.Modules.UpdateRange(modules);
            await _context.SaveEntitiesAsync();
            return await _context.Modules.Where(m => m.ModuleGroupId == modules[0].ModuleGroupId).ToListAsync();
        }
        #endregion
    }
}
