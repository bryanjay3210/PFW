using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.RolesAndAccess;
using Domain.DomainModel.Interface;
using Domain.DomainModel.Interface.RolesAndAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DataContext _context;
        private readonly IRolePermissionRepository _rolePermissionRepository;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public RoleRepository(DataContext context, IRolePermissionRepository rolePermissionRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _rolePermissionRepository = rolePermissionRepository;
        }

        #region Get Data
        public async Task<List<Role>> GetRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            foreach(var role in roles)
            {
                role.RolePermissions = _rolePermissionRepository.GetRolePermissions().Result
                    .Where(rp => rp.RoleId == role.Id && rp.IsActive == true)
                    .OrderBy(r => r.RoleId)
                    .OrderBy(mg => mg.ModuleGroupId)
                    .ToList();
            }
            return roles;
        }

        public async Task<Role?> GetRole(int roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);
            
            if (role != null)
            {
                role.RolePermissions = _rolePermissionRepository.GetRolePermissions().Result
                    .Where(rp => rp.RoleId == role.Id && rp.IsActive == true)
                    .OrderBy(r => r.RoleId)
                    .OrderBy(mg => mg.ModuleGroupId)
                    .ToList();
            }

            return role;
        }
        #endregion

        #region Save Data
        public async Task<List<Role>> Create(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveEntitiesAsync();
            var newId = role.Id;
            
            role.RolePermissions.ForEach(rp => 
            {
                rp.RoleId = newId;
            });

            _context.RolePermissions.AddRange(role.RolePermissions);
            await _context.SaveEntitiesAsync();
            
            return await _context.Roles.ToListAsync();
        }

        public async Task<List<Role>> Update(Role role)
        {
            _context.Roles.Update(role);
            await _context.SaveEntitiesAsync();

            var updateRolePermissions = role.RolePermissions.Where(rp => rp.Id > 0).ToList();
            _context.RolePermissions.UpdateRange(updateRolePermissions);
            await _context.SaveEntitiesAsync();

            var createRolePermissions = role.RolePermissions.Where(rp => rp.Id == 0).ToList();
            _context.RolePermissions.AddRange(createRolePermissions);
            await _context.SaveEntitiesAsync();
            
            return await _context.Roles.ToListAsync();
        }

        public async Task<List<Role>> Delete(List<int> roleIds)
        {
            var roles = _context.Roles.Where(a => roleIds.Contains(a.Id)).ToList();
            _context.Roles.RemoveRange(roles);
            await _context.SaveEntitiesAsync();
            return await _context.Roles.ToListAsync();
        }

        public async Task<List<Role>> SoftDelete(List<int> roleIds)
        {
            var roles = _context.Roles.Where(a => roleIds.Contains(a.Id)).ToList();
            roles.ForEach(a => { a.IsDeleted = true; });

            _context.Roles.UpdateRange(roles);
            await _context.SaveEntitiesAsync();
            return await _context.Roles.ToListAsync();
        }
        #endregion
    }
}
