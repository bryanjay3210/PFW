using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public CategoryRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<Category>> GetCategories()
        {
            var result = await _context.Categories.OrderBy(c => c.Description).ToListAsync();
            
            foreach (var category in result)
            {
                category.Sequences = await _context.Sequences.Where(sequence => sequence.CatId == category.CatId).ToListAsync();
            }

            return result;
        }

        public async Task<Category?> GetCategory(int categoryId)
        {
            var result = await _context.Categories.FindAsync(categoryId);
            if (result == null)
                return null;

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<Category>> Create(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveEntitiesAsync();
            return await _context.Categories.ToListAsync();
        }

        public async Task<List<Category>> Update(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveEntitiesAsync();
            return await _context.Categories.ToListAsync();
        }

        public async Task<List<Category>> Delete(List<int> categoryIds)
        {
            var categorys = _context.Categories.Where(a => categoryIds.Contains(a.Id)).ToList();
            _context.Categories.RemoveRange(categorys);
            await _context.SaveEntitiesAsync();
            return await _context.Categories.ToListAsync();
        }

        public async Task<List<Category>> SoftDelete(List<int> categoryIds)
        {
            var categorys = _context.Categories.Where(a => categoryIds.Contains(a.Id)).ToList();
            categorys.ForEach(a => { a.IsDeleted = true; });

            _context.Categories.UpdateRange(categorys);
            await _context.SaveEntitiesAsync();
            return await _context.Categories.ToListAsync();
        }
        #endregion
    }
}
