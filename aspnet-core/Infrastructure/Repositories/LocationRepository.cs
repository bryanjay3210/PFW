using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public LocationRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<Location>> GetLocations()
        {
            return await _context.Locations.ToListAsync();
        }


        public async Task<List<Location>> GetLocationsByCustomerId(int customerId)
        {
            var result = await _context.Locations.Where(e => e.CustomerId == customerId && e.IsDeleted == false).ToListAsync();
            foreach (var location in result)
            {
                var loc = await _context.Zones.Where(e => e.ZipCode.Trim() == location.ZipCode.Trim()).FirstOrDefaultAsync();
                location.Zone = loc == null ? "" : loc.BinCode;
            }

            return result;
        }

        public async Task<List<LocationDTO>> GetLocationsList(int customerId)
        {
            var result = new List<LocationDTO>();
            var locations =  await _context.Locations.Where(e => e.CustomerId == customerId && e.IsDeleted == false).ToListAsync();

            foreach (var location in locations)
            {
                var contact = await _context.Contacts.Where(e => e.LocationId == location.Id && e.IsDeleted == false).FirstOrDefaultAsync();
                var zone = await _context.Zones.Where(e => e.ZipCode.Trim().ToLower() == location.ZipCode.Trim().ToLower()).FirstOrDefaultAsync();

                var locationDTO = new LocationDTO()
                {
                    AddressLine1 = location.AddressLine1,
                    AddressLine2 = location.AddressLine2,
                    City = location.City,
                    ContactName = contact == null ? "" : contact.ContactName,
                    Country = location.Country,
                    CustomerId = customerId,
                    Email = location.Email,
                    FaxNumber = location.FaxNumber,
                    Id = location.Id,
                    IsDeleted = location.IsDeleted,
                    LocationCode = location.LocationCode,
                    LocationName = location.LocationName,
                    LocationTypeId = location.LocationTypeId,
                    PhoneNumber = location.PhoneNumber,
                    State = location.State,
                    ZipCode = location.ZipCode,
                    Zone = zone == null ? "" : zone.BinCode
                };

                result.Add(locationDTO);
            }

            return result;
        }

        public async Task<Location?> GetLocation(int locationId)
        {
            var result = await _context.Locations.FindAsync(locationId);
            if (result == null)
                return null;

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<Location>> Create(Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveEntitiesAsync();
            return await GetLocationsByCustomerId(location.CustomerId);
        }

        public async Task<List<Location>> Update(Location location)
        {
            _context.Locations.Update(location);
            await _context.SaveEntitiesAsync();
            return await GetLocationsByCustomerId(location.CustomerId);
        }

        public async Task<List<Location>> Delete(List<int> locationIds)
        {
            var locations = _context.Locations.Where(a => locationIds.Contains(a.Id)).ToList();
            _context.Locations.RemoveRange(locations);
            await _context.SaveEntitiesAsync();
            return await GetLocationsByCustomerId(locations[0].CustomerId);
        }

        public async Task<List<Location>> SoftDelete(List<int> locationIds)
        {
            var locations = _context.Locations.Where(a => locationIds.Contains(a.Id)).ToList();
            locations.ForEach(c => { c.IsDeleted = true; });

            _context.Locations.UpdateRange(locations);
            await _context.SaveEntitiesAsync();
            return await GetLocationsByCustomerId(locations[0].CustomerId);
        }
        #endregion
    }
}
