using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.RolesAndAccess;
using Domain.DomainModel.Interface;
using Domain.DomainModel.Interface.RolesAndAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly DataContext _context;
        private readonly IModuleGroupRepository _moduleGroupRepository;
        private readonly IModuleRepository _moduleRepository;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public RolePermissionRepository(DataContext context, IModuleGroupRepository moduleGroupRepository, IModuleRepository moduleRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _moduleGroupRepository = moduleGroupRepository;
            _moduleRepository = moduleRepository;
        }

        #region Get Data
        public async Task<List<RolePermission>> GetRolePermissions()
        {
            var result = await _context.RolePermissions.Where(rp => rp.IsActive == true).ToListAsync();

            foreach (var rolePermission in result)
            {
                rolePermission.ModuleGroup = await _moduleGroupRepository.GetModuleGroup(rolePermission.ModuleGroupId);
                rolePermission.Module = await _moduleRepository.GetModule(rolePermission.ModuleId);
            }

            return result;
        }

        public async Task<RolePermission?> GetRolePermission(int rolePermissionId)
        {
            var result = await _context.RolePermissions.FindAsync(rolePermissionId);
            if (result != null)
            {
                result.ModuleGroup = await _moduleGroupRepository.GetModuleGroup(result.ModuleGroupId);
                result.Module = await _moduleRepository.GetModule(result.ModuleId);
            }

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<RolePermission>> Create(RolePermission rolePermission)
        {
            _context.RolePermissions.Add(rolePermission);
            await _context.SaveEntitiesAsync();
            return await _context.RolePermissions.ToListAsync();
        }

        public async Task<List<RolePermission>> Update(RolePermission rolePermission)
        {
            _context.RolePermissions.Update(rolePermission);
            await _context.SaveEntitiesAsync();
            return await _context.RolePermissions.ToListAsync();
        }

        public async Task<List<RolePermission>> Delete(List<int> rolePermissionIds)
        {
            var rolePermissions = _context.RolePermissions.Where(a => rolePermissionIds.Contains(a.Id)).ToList();
            _context.RolePermissions.RemoveRange(rolePermissions);
            await _context.SaveEntitiesAsync();
            return await _context.RolePermissions.ToListAsync();
        }

        public async Task<List<RolePermission>> SoftDelete(List<int> rolePermissionIds)
        {
            var rolePermissions = _context.RolePermissions.Where(a => rolePermissionIds.Contains(a.Id)).ToList();
            rolePermissions.ForEach(a => { a.IsDeleted = true; });

            _context.RolePermissions.UpdateRange(rolePermissions);
            await _context.SaveEntitiesAsync();
            return await _context.RolePermissions.ToListAsync();
        }
        #endregion
    }
}
