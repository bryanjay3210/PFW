using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Infrastucture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SequenceController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly ISequenceRepository _sequenceRepository;

        public SequenceController(DataContext dataContext, ISequenceRepository sequenceRepository)
        {
            _dataContext = dataContext;
            _sequenceRepository = sequenceRepository;
        }

        #region Get Data
        [HttpGet("GetSequences")]
        public async Task<ActionResult<List<Sequence>>> GetSequences()
        {
            return Ok(await _sequenceRepository.GetSequences());
        }

        [HttpGet("GetSequencesByCategoryId")]
        public async Task<ActionResult<List<Sequence>>> GetSequencesByCategoryId(int categoryId)
        {
            return Ok(await _sequenceRepository.GetSequencesByCategoryId(categoryId));
        }

        [HttpGet("GetSequenceById")]
        public async Task<ActionResult<Sequence>> GetSequenceById(int sequenceId)
        {
            var sequence = await _sequenceRepository.GetSequence(sequenceId);
            if (sequence == null)
                return NotFound("Sequence not found!");
            return Ok(sequence);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateSequence")]
        public async Task<ActionResult<List<Sequence>>> CreateSequence(Sequence sequence)
        {
            var sequenceList = await _sequenceRepository.Create(sequence);
            return Ok(sequenceList);
        }

        [HttpPut("UpdateSequence")]
        public async Task<ActionResult<List<Sequence>>> UpdateSequence(Sequence sequence)
        {
            var sequenceList = await _sequenceRepository.Update(sequence);
            return Ok(sequenceList);
        }

        [HttpDelete("DeleteSequence")]
        public async Task<ActionResult<List<Sequence>>> DeleteSequence(List<int> sequenceIds)
        {
            var sequenceList = await _sequenceRepository.Delete(sequenceIds);
            return Ok(sequenceList);
        }
        #endregion
    }
}
