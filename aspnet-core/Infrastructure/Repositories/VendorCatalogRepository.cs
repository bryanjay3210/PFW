using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class VendorCatalogRepository : IVendorCatalogRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public VendorCatalogRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<VendorCatalog>> GetVendorCatalogs()
        {
            return await _context.VendorCatalogs.ToListAsync();
        }

        public async Task<VendorCatalog?> GetVendorCatalog(int vendorCatalogId)
        {
            var result = await _context.VendorCatalogs.FindAsync(vendorCatalogId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<List<VendorCatalog>> GetVendorCatalogsByPartsLinkNumbers(List<string> partsLinkNumbers)
        {
            try
            {
                var result = await _context.VendorCatalogs.Where(e => e.IsDeleted == false && partsLinkNumbers.Contains(e.PartsLinkNumber)).OrderBy(e => e.PartsLinkNumber).ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                return new List<VendorCatalog>();
            }
            
        }
        #endregion

        #region Save Data
        public async Task<List<VendorCatalog>> Create(VendorCatalog vendorCatalog)
        {
            _context.VendorCatalogs.Add(vendorCatalog);
            await _context.SaveEntitiesAsync();
            return await _context.VendorCatalogs.ToListAsync();
        }

        public async Task<List<VendorCatalog>> Update(VendorCatalog vendorCatalog)
        {
            _context.VendorCatalogs.Update(vendorCatalog);
            await _context.SaveEntitiesAsync();
            return await _context.VendorCatalogs.ToListAsync();
        }

        public async Task<List<VendorCatalog>> Delete(List<int> vendorCatalogIds)
        {
            var vendorCatalogs = _context.VendorCatalogs.Where(a => vendorCatalogIds.Contains(a.Id)).ToList();
            _context.VendorCatalogs.RemoveRange(vendorCatalogs);
            await _context.SaveEntitiesAsync();
            return await _context.VendorCatalogs.ToListAsync();
        }

        public async Task<List<VendorCatalog>> SoftDelete(List<int> vendorCatalogIds)
        {
            var vendorCatalogs = _context.VendorCatalogs.Where(a => vendorCatalogIds.Contains(a.Id)).ToList();
            vendorCatalogs.ForEach(a => { a.IsDeleted = true; });

            _context.VendorCatalogs.UpdateRange(vendorCatalogs);
            await _context.SaveEntitiesAsync();
            return await _context.VendorCatalogs.ToListAsync();
        }

        public async Task<List<VendorCatalog>> CreateByProduct(VendorCatalog vendorCatalog, List<string> partsLinkNumbers)
        {
            _context.VendorCatalogs.Add(vendorCatalog);
            await _context.SaveEntitiesAsync();
            return await GetVendorCatalogsByPartsLinkNumbers(partsLinkNumbers);
        }

        public async Task<List<VendorCatalog>> UpdateByProduct(VendorCatalog vendorCatalog, List<string> partsLinkNumbers)
        {
            _context.VendorCatalogs.Update(vendorCatalog);
            await _context.SaveEntitiesAsync();
            return await GetVendorCatalogsByPartsLinkNumbers(partsLinkNumbers);
        }
        #endregion
    }
}
