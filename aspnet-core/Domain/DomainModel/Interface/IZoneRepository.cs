using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface IZoneRepository : IRepository<Zone>
    {
        #region Get Data
        Task<List<Zone>> GetZones();
        Task<Zone?> GetZone(int zoneId);
        Task<Zone?> GetZoneByZipCode(string zipCode);
        #endregion

        #region Save Data
        Task<List<Zone>> Create(Zone zone);
        Task<List<Zone>> Update(Zone zone);
        Task<List<Zone>> Delete(List<int> zoneIds);
        Task<List<Zone>> SoftDelete(List<int> zoneIds);
        #endregion
    }
}
