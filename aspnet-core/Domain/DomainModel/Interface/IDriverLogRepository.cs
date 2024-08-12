using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO.Paginated;

namespace Domain.DomainModel.Interface
{
    public interface IDriverLogRepository : IRepository<DriverLog>
    {
        #region Get Data
        Task<List<DriverLog>> GetDriverLogs();
        Task<DriverLog?> GetDriverLog(int driverLogId);

        Task<PaginatedListDTO<DriverLog>> GetDriverLogsPaginated(int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search);
        Task<PaginatedListDTO<DriverLog>> GetDriverLogsByDatePaginated(int pageSize, int pageIndex, DateTime fromDate, DateTime toDate);
        #endregion

        #region Save Data
        Task<bool> Create(DriverLog driverLog);
        Task<List<DriverLog>> Update(DriverLog driverLog);
        Task<List<DriverLog>> Delete(List<int> driverLogIds);
        Task<List<DriverLog>> SoftDelete(List<int> driverLogIds);
        #endregion
    }
}
