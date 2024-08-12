using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface IPriceLevelRepository : IRepository<PriceLevel>
    {
        #region Get Data
        Task<List<PriceLevel>> GetPriceLevels();
        Task<PriceLevel?> GetPriceLevel(int priceLevelId);
        #endregion

        #region Save Data
        Task<List<PriceLevel>> Create(PriceLevel priceLevel);
        Task<List<PriceLevel>> Update(PriceLevel priceLevel);
        Task<List<PriceLevel>> Delete(List<int> priceLevelIds);
        Task<List<PriceLevel>> SoftDelete(List<int> priceLevelIds);
        #endregion
    }
}
