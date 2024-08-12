using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class SequenceRepository : ISequenceRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public SequenceRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<Sequence>> GetSequences()
        {
            var result = await _context.Sequences.OrderBy(e => e.CategoryGroupDescription).ToListAsync();
            return result;
        }

        public async Task<List<Sequence>> GetSequencesByCategoryId(int categoryId)
        {
            return await _context.Sequences.Where(s => s.CategoryId == categoryId).ToListAsync();
        }

        public async Task<Sequence?> GetSequence(int sequenceId)
        {
            var result = await _context.Sequences.FindAsync(sequenceId);
            if (result == null)
                return null;

            return result;
        }
        #endregion

        #region Save Data
        public async Task<List<Sequence>> Create(Sequence sequence)
        {
            _context.Sequences.Add(sequence);
            await _context.SaveEntitiesAsync();
            return await _context.Sequences.ToListAsync();
        }

        public async Task<List<Sequence>> Update(Sequence sequence)
        {
            _context.Sequences.Update(sequence);
            await _context.SaveEntitiesAsync();
            return await _context.Sequences.ToListAsync();
        }

        public async Task<List<Sequence>> Delete(List<int> sequenceIds)
        {
            var sequences = _context.Sequences.Where(a => sequenceIds.Contains(a.Id)).ToList();
            _context.Sequences.RemoveRange(sequences);
            await _context.SaveEntitiesAsync();
            return await _context.Sequences.ToListAsync();
        }

        public async Task<List<Sequence>> SoftDelete(List<int> sequenceIds)
        {
            var sequences = _context.Sequences.Where(a => sequenceIds.Contains(a.Id)).ToList();
            sequences.ForEach(a => { a.IsDeleted = true; });

            _context.Sequences.UpdateRange(sequences);
            await _context.SaveEntitiesAsync();
            return await _context.Sequences.ToListAsync();
        }
        #endregion
    }
}
