using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO.Paginated;
using Domain.DomainModel.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DriverLogController : ControllerBase
    {
        private readonly IDriverLogRepository _driverLogRepository;
        
        public DriverLogController(IDriverLogRepository driverLogRepository)
        {
            _driverLogRepository = driverLogRepository;
        }

        #region Get Data
        [HttpGet("GetDriverLogs")] 
        public async Task<ActionResult<List<DriverLog>>> GetDriverLogs()
        {
            return Ok(await _driverLogRepository.GetDriverLogs());
        }

        [HttpGet("GetDriverLogById")]
        public async Task<ActionResult<DriverLog>> GetDriverLogById(int driverLogId)
        {
            var driverLog = await _driverLogRepository.GetDriverLog(driverLogId);
            if (driverLog == null)
                return NotFound("DriverLog not found!");
            return Ok(driverLog);
        }

        [HttpGet("GetDriverLogsPaginated")]
        public async Task<ActionResult<PaginatedListDTO<DriverLog>>> GetDriverLogsPaginated(
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "DriverLogNumber",
            [FromQuery] string? sortOrder = "DESC",
        [FromQuery] string? search = ""
            )
        {
            var result = await _driverLogRepository.GetDriverLogsPaginated(pageSize, pageIndex, sortColumn, sortOrder, search);
            return Ok(result);
        }

        [HttpGet("GetDriverLogsByDatePaginated")]
        public async Task<ActionResult<PaginatedListDTO<DriverLog>>> GetDriverLogsByDatePaginated( int pageSize, int pageIndex, DateTime fromDate, DateTime toDate)
        {
            var result = await _driverLogRepository.GetDriverLogsByDatePaginated(pageSize, pageIndex, fromDate, toDate);
            return Ok(result);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateDriverLog")]
        public async Task<ActionResult<bool>> CreateDriverLog(DriverLog driverLog)
        {
            var result = await _driverLogRepository.Create(driverLog);
            return Ok(result);
        }

        [HttpPut("UpdateDriverLog")]
        public async Task<ActionResult<List<DriverLog>>> UpdateDriverLog(DriverLog driverLog)
        {
            var driverLogList = await _driverLogRepository.Update(driverLog);
            
            //if (driverLogList == null)
            //    return NotFound("Error encountered while updating driverLog!");

            return Ok(driverLogList);
        }

        [HttpDelete("DeleteDriverLog")]
        public async Task<ActionResult<List<DriverLog>>> DeleteDriverLog(List<int> driverLogIds)
        {
            var driverLogList = await _driverLogRepository.Delete(driverLogIds);
            
            //if (driverLogList == null)
            //    return NotFound("Error encountered when deleting driverLog!");

            return Ok(driverLogList);
        }
        #endregion
    }
}
