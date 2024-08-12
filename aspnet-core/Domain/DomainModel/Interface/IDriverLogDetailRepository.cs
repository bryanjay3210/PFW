using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface IDriverLogDetailRepository : IRepository<DriverLogDetail>
    {
        #region Get Data
        Task<List<DriverLogDetail>> GetDriverLogDetails();
        Task<DriverLogDetail?> GetDriverLogDetail(int driverLogDetailId);
        #endregion

        #region Save Data
        Task<List<DriverLogDetail>> Create(DriverLogDetail driverLogDetail);
        Task<List<DriverLogDetail>> Update(DriverLogDetail driverLogDetail);
        Task<List<DriverLogDetail>> Delete(List<int> driverLogDetailIds);
        Task<List<DriverLogDetail>> SoftDelete(List<int> driverLogDetailIds);
        #endregion
    }
}
