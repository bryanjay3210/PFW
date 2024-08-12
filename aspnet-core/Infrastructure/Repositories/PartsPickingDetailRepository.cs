using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class PartsPickingDetailRepository : IPartsPickingDetailRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public PartsPickingDetailRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<PartsPickingDetail>> GetPartsPickingDetails(int partsPickingId)
        {
            return await _context.PartsPickingDetails.ToListAsync();
        }

        public async Task<PartsPickingDetail?> GetPartsPickingDetail(int partsPickingDetailId)
        {
            var result = await _context.PartsPickingDetails.FindAsync(partsPickingDetailId);
            if (result == null)
                return null;

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<PartsPickingDetail>> Create(PartsPickingDetail partsPickingDetail)
        {
            _context.PartsPickingDetails.Add(partsPickingDetail);
            await _context.SaveEntitiesAsync();
            return await _context.PartsPickingDetails.ToListAsync();
        }

        public async Task<List<PartsPickingDetail>> Update(PartsPickingDetail partsPickingDetail)
        {
            _context.PartsPickingDetails.Update(partsPickingDetail);
            await _context.SaveEntitiesAsync();
            return await _context.PartsPickingDetails.ToListAsync();
        }

        public async Task<List<PartsPickingDetail>> Delete(List<int> partsPickingDetailIds)
        {
            var partsPickingDetails = _context.PartsPickingDetails.Where(a => partsPickingDetailIds.Contains(a.Id)).ToList();
            _context.PartsPickingDetails.RemoveRange(partsPickingDetails);
            await _context.SaveEntitiesAsync();
            return await _context.PartsPickingDetails.ToListAsync();
        }

        public async Task<List<PartsPickingDetail>> SoftDelete(List<int> partsPickingDetailIds)
        {
            var partsPickingDetails = _context.PartsPickingDetails.Where(a => partsPickingDetailIds.Contains(a.Id)).ToList();
            partsPickingDetails.ForEach(a => { a.IsDeleted = true; });

            _context.PartsPickingDetails.UpdateRange(partsPickingDetails);
            await _context.SaveEntitiesAsync();
            return await _context.PartsPickingDetails.ToListAsync();
        }
        #endregion
    }
}
