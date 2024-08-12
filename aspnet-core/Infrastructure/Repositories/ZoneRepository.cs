using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class ZoneRepository : IZoneRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public ZoneRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<Zone?> GetZone(int zoneId)
        {
            var result = await _context.Zones.FindAsync(zoneId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<List<Zone>> GetZones()
        {
            return await _context.Zones.ToListAsync();
        }

        public async Task<Zone?> GetZoneByZipCode(string zipCode)
        {
            var result = await _context.Zones.Where(e => e.ZipCode.Trim() == zipCode).ToListAsync();
            if (result == null)
                return null;

            return result.FirstOrDefault();
        }
        #endregion

        #region Save Data
        public async Task<List<Zone>> Create(Zone zone)
        {
            _context.Zones.Add(zone);
            await _context.SaveEntitiesAsync();
            return await _context.Zones.ToListAsync();
        }

        public async Task<List<Zone>> Update(Zone zone)
        {
            _context.Zones.Update(zone);
            await _context.SaveEntitiesAsync();
            return await _context.Zones.ToListAsync();
        }

        public async Task<List<Zone>> Delete(List<int> zoneIds)
        {
            var zones = _context.Zones.Where(a => zoneIds.Contains(a.Id)).ToList();
            _context.Zones.RemoveRange(zones);
            await _context.SaveEntitiesAsync();
            return await _context.Zones.ToListAsync();
        }

        public async Task<List<Zone>> SoftDelete(List<int> zoneIds)
        {
            var zones = _context.Zones.Where(a => zoneIds.Contains(a.Id)).ToList();
            zones.ForEach(c => { c.IsDeleted = true; });

            _context.Zones.UpdateRange(zones);
            await _context.SaveEntitiesAsync();
            return await _context.Zones.ToListAsync();
        }
        #endregion
    }
}
