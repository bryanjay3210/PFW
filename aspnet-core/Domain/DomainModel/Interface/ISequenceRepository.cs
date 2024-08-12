using Domain.DomainModel.Entity;

namespace Domain.DomainModel.Interface
{
    public interface ISequenceRepository : IRepository<Sequence>
    {
        #region Get Data
        Task<List<Sequence>> GetSequences();
        Task<List<Sequence>> GetSequencesByCategoryId(int categoryId);
        Task<Sequence?> GetSequence(int sequenceId);
        #endregion

        #region Save Data
        Task<List<Sequence>> Create(Sequence sequence);
        Task<List<Sequence>> Update(Sequence sequence);
        Task<List<Sequence>> Delete(List<int> sequenceIds);
        Task<List<Sequence>> SoftDelete(List<int> sequenceIds);
        #endregion
    }
}
