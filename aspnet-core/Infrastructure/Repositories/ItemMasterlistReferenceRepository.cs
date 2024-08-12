using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class ItemMasterlistReferenceRepository : IItemMasterlistReferenceRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public ItemMasterlistReferenceRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<ItemMasterlistReference>> GetItemMasterlistReferences()
        {
            return await _context.ItemMasterlistReferences.ToListAsync();
        }

        public async Task<List<ItemMasterlistReference>> GetItemMasterlistReferencesByProductId(int productId)
        {
            return await _context.ItemMasterlistReferences.Where(r => r.ProductId == productId && r.IsDeleted == false).ToListAsync();
        }

        public async Task<ItemMasterlistReference?> GetItemMasterlistReference(int itemMasterlistReferenceId)
        {
            var result = await _context.ItemMasterlistReferences.FindAsync(itemMasterlistReferenceId);
            if (result == null)
                return null;

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<ItemMasterlistReference>> Create(ItemMasterlistReference itemMasterlistReference)
        {
            _context.ItemMasterlistReferences.Add(itemMasterlistReference);
            await _context.SaveEntitiesAsync();
            return await _context.ItemMasterlistReferences.ToListAsync();
        }

        public async Task<List<ItemMasterlistReference>> Update(ItemMasterlistReference itemMasterlistReference)
        {
            _context.ItemMasterlistReferences.Update(itemMasterlistReference);
            await _context.SaveEntitiesAsync();
            return await _context.ItemMasterlistReferences.ToListAsync();
        }

        public async Task<List<ItemMasterlistReference>> Delete(List<int> itemMasterlistReferenceIds)
        {
            var itemMasterlistReferences = _context.ItemMasterlistReferences.Where(a => itemMasterlistReferenceIds.Contains(a.Id)).ToList();
            _context.ItemMasterlistReferences.RemoveRange(itemMasterlistReferences);
            await _context.SaveEntitiesAsync();
            return await _context.ItemMasterlistReferences.ToListAsync();
        }

        public async Task<List<ItemMasterlistReference>> SoftDelete(List<int> itemMasterlistReferenceIds)
        {
            var itemMasterlistReferences = _context.ItemMasterlistReferences.Where(a => itemMasterlistReferenceIds.Contains(a.Id)).ToList();
            itemMasterlistReferences.ForEach(a => { a.IsDeleted = true; });

            _context.ItemMasterlistReferences.UpdateRange(itemMasterlistReferences);
            await _context.SaveEntitiesAsync();
            return await _context.ItemMasterlistReferences.ToListAsync();
        }

        public async Task<List<ItemMasterlistReference>> CreateByProduct(ItemMasterlistReference itemMasterlistReference)
        {
            _context.ItemMasterlistReferences.Add(itemMasterlistReference);
            await _context.SaveEntitiesAsync();
            return await GetItemMasterlistReferencesByProductId(itemMasterlistReference.ProductId);
        }

        public async Task<List<ItemMasterlistReference>> UpdateByProduct(ItemMasterlistReference itemMasterlistReference)
        {
            _context.ItemMasterlistReferences.Update(itemMasterlistReference);
            await _context.SaveEntitiesAsync();
            return await GetItemMasterlistReferencesByProductId(itemMasterlistReference.ProductId);
        }

        public async Task<bool> RemoveExistingIsMainPartsLink(int itemMasterlistReferenceId, int productId)
        {
            var itemMasterlistReference = await _context.ItemMasterlistReferences.FirstOrDefaultAsync(a => a.Id != itemMasterlistReferenceId && a.ProductId == productId && a.IsMainPartsLink);
            if (itemMasterlistReference != null)
            {
                itemMasterlistReference.IsMainPartsLink = false;
                _context.ItemMasterlistReferences.Update(itemMasterlistReference);
                await _context.SaveEntitiesAsync();
                
            }
            return true;
        }

        public async Task<bool> RemoveExistingIsMainOEM(int itemMasterlistReferenceId, int productId)
        {
            var itemMasterlistReference = await _context.ItemMasterlistReferences.FirstOrDefaultAsync(a => a.Id != itemMasterlistReferenceId && a.ProductId == productId && a.IsMainOEM);
            if (itemMasterlistReference != null)
            {
                itemMasterlistReference.IsMainOEM = false;
                _context.ItemMasterlistReferences.Update(itemMasterlistReference);
                await _context.SaveEntitiesAsync();
            }
            return true;
        }
        #endregion
    }
}
