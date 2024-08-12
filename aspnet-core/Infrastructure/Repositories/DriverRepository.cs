using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class DriverRepository : IDriverRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public DriverRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<Driver>> GetDrivers()
        {
            return await _context.Drivers.ToListAsync();
        }

        public async Task<Driver?> GetDriver(int driverId)
        {
            var result = await _context.Drivers.FindAsync(driverId);
            if (result == null)
                return null;

            return result;
        }
        public async Task<Driver?> GetDriverByDriverNumber(string driverNumber)
        {
            var result = await _context.Drivers.FirstOrDefaultAsync(e => e.DriverNumberString.Trim() == driverNumber.Trim());
            if (result == null)
                return null;

            return result;
        }

        #endregion

        #region Save Data
        public async Task<List<Driver>> Create(Driver driver)
        {
            _context.Drivers.Add(driver);
            await _context.SaveEntitiesAsync();
            return await _context.Drivers.ToListAsync();
        }

        public async Task<List<Driver>> Update(Driver driver)
        {
            _context.Drivers.Update(driver);
            await _context.SaveEntitiesAsync();
            return await _context.Drivers.ToListAsync();
        }

        public async Task<List<Driver>> Delete(List<int> driverIds)
        {
            var drivers = _context.Drivers.Where(a => driverIds.Contains(a.Id)).ToList();
            _context.Drivers.RemoveRange(drivers);
            await _context.SaveEntitiesAsync();
            return await _context.Drivers.ToListAsync();
        }

        public async Task<List<Driver>> SoftDelete(List<int> driverIds)
        {
            var drivers = _context.Drivers.Where(a => driverIds.Contains(a.Id)).ToList();
            drivers.ForEach(a => { a.IsDeleted = true; });

            _context.Drivers.UpdateRange(drivers);
            await _context.SaveEntitiesAsync();
            return await _context.Drivers.ToListAsync();
        }

        #endregion
    }
}
