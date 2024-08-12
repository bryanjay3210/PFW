using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface IDriverRepository : IRepository<Driver>
    {
        #region Get Data
        Task<List<Driver>> GetDrivers();
        Task<Driver?> GetDriver(int driverId);

        Task<Driver?> GetDriverByDriverNumber(string driverNumber);
        #endregion

        #region Save Data
        Task<List<Driver>> Create(Driver driver);
        Task<List<Driver>> Update(Driver driver);
        Task<List<Driver>> Delete(List<int> driverIds);
        Task<List<Driver>> SoftDelete(List<int> driverIds);
        #endregion
    }
}
