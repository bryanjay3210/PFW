using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class PartsCatalogRepository : IPartsCatalogRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public PartsCatalogRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<PartsCatalog>> GetPartsCatalogs()
        {
            return await _context.PartsCatalogs.ToListAsync();
        }

        public async Task<List<PartsCatalog>> GetPartsCatalogsByProductId(int productId)
        {
            return await _context.PartsCatalogs.Where(pc => pc.ProductId == productId).ToListAsync();
        }

        public async Task<PartsCatalog?> GetPartsCatalog(int partsCatalogId)
        {
            var result = await _context.PartsCatalogs.FindAsync(partsCatalogId);
            if (result == null)
                return null;

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<PartsCatalog>> Create(PartsCatalog partsCatalog)
        {
            _context.PartsCatalogs.Add(partsCatalog);
            await _context.SaveEntitiesAsync();
            return await _context.PartsCatalogs.Where(pc => pc.ProductId == partsCatalog.ProductId).ToListAsync();
        }

        public async Task<List<PartsCatalog>> Update(PartsCatalog partsCatalog)
        {
            _context.PartsCatalogs.Update(partsCatalog);
            await _context.SaveEntitiesAsync();
            return await _context.PartsCatalogs.Where(pc => pc.ProductId == partsCatalog.ProductId).ToListAsync();
        }

        public async Task<List<PartsCatalog>> Delete(List<int> partsCatalogIds)
        {
            var partsCatalogs = _context.PartsCatalogs.Where(a => partsCatalogIds.Contains(a.Id)).ToList();
            _context.PartsCatalogs.RemoveRange(partsCatalogs);
            await _context.SaveEntitiesAsync();
            return await _context.PartsCatalogs.Where(pc => pc.ProductId == partsCatalogs[0].ProductId).ToListAsync();
        }

        public async Task<List<PartsCatalog>> SoftDelete(List<int> partsCatalogIds)
        {
            var partsCatalogs = _context.PartsCatalogs.Where(a => partsCatalogIds.Contains(a.Id)).ToList();
            partsCatalogs.ForEach(a => { a.IsDeleted = true; });

            _context.PartsCatalogs.UpdateRange(partsCatalogs);
            await _context.SaveEntitiesAsync();
            return await _context.PartsCatalogs.Where(pc => pc.ProductId == partsCatalogs[0].ProductId).ToListAsync();
        }
        #endregion
    }
}
