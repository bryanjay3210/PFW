using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;

namespace Domain.DomainModel.Interface
{
    public interface ILocationRepository : IRepository<Location>
    {
        #region Get Data
        Task<List<Location>> GetLocations();
        Task<List<Location>> GetLocationsByCustomerId(int customerId);
        Task<List<LocationDTO>> GetLocationsList(int customerId);
        Task<Location?> GetLocation(int locationId);
        #endregion

        #region Save Data
        Task<List<Location>> Create(Location location);
        Task<List<Location>> Update(Location location);
        Task<List<Location>> Delete(List<int> locationIds);
        Task<List<Location>> SoftDelete(List<int> locationIds);
        #endregion
    }
}
