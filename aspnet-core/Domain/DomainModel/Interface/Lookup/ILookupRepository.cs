using Domain.DomainModel.Entity.DTO;
using System.Linq.Expressions;

namespace Domain.DomainModel.Interface
{
    public interface ILookupRepository
    {
        #region Get Data
        Task<List<TEntity>> GetLookupData<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        Task<TEntity?> GetLookupDataById<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;

        Task<List<YearDTO>> GetYearsListDistinct();
        Task<List<MakeDTO>> GetMakesListDistinct();
        Task<List<ModelDTO>> GetModelsListDistinct();
        Task<List<CategoryDTO>> GetCategoriesListDistinct();
        Task<List<SequenceDTO>> GetSequencesListDistinct();

        Task<List<MakeDTO>> GetMakesListByYear(ProductFilterDTO productFilterDTO);
        Task<List<ModelDTO>> GetModelsListByMake(ProductFilterDTO productFilterDTO);
        Task<List<CategoryDTO>> GetCategoriesListByModel(ProductFilterDTO productFilterDTO);
        Task<List<SequenceDTO>> GetSequencesListByCategoryId(ProductFilterDTO productFilterDTO);
        #endregion

        #region Save Data
        Task<TEntity> CreateLookupData<TEntity>(TEntity lookup) where TEntity : class;
        Task<TEntity> UpdateLookupData<TEntity>(TEntity lookup) where TEntity : class;
        #endregion
    }
}
