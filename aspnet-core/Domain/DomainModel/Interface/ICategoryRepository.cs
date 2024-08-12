using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface ICategoryRepository : IRepository<Category>
    {
        #region Get Data
        Task<List<Category>> GetCategories();
        Task<Category?> GetCategory(int categoryId);
        #endregion

        #region Save Data
        Task<List<Category>> Create(Category category);
        Task<List<Category>> Update(Category category);
        Task<List<Category>> Delete(List<int> categoryIds);
        Task<List<Category>> SoftDelete(List<int> categoryIds);
        #endregion
    }
}
