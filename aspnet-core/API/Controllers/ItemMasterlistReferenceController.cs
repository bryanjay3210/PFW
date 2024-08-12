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
    public class ItemMasterlistReferenceController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IItemMasterlistReferenceRepository _itemMasterlistReferenceRepository;

        public ItemMasterlistReferenceController(DataContext dataContext, IItemMasterlistReferenceRepository itemMasterlistReferenceRepository)
        {
            _dataContext = dataContext;
            _itemMasterlistReferenceRepository = itemMasterlistReferenceRepository;
        }

        #region Get Data
        [HttpGet("GetItemMasterlistReferences")]
        public async Task<ActionResult<List<ItemMasterlistReference>>> GetItemMasterlistReferences()
        {
            return Ok(await _itemMasterlistReferenceRepository.GetItemMasterlistReferences());
        }

        [HttpGet("GetItemMasterlistReferencesByProductId")]
        public async Task<ActionResult<List<ItemMasterlistReference>>> GetItemMasterlistReferencesByProductId(int productId)
        {
            return Ok(await _itemMasterlistReferenceRepository.GetItemMasterlistReferencesByProductId(productId));
        }

        [HttpGet("GetItemMasterlistReferenceById")]
        public async Task<ActionResult<ItemMasterlistReference>> GetItemMasterlistReferenceById(int itemMasterlistReferenceId)
        {
            var itemMasterlistReference = await _itemMasterlistReferenceRepository.GetItemMasterlistReference(itemMasterlistReferenceId);
            if (itemMasterlistReference == null)
                return NotFound("Item Masterlist Reference not found!");
            return Ok(itemMasterlistReference);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateItemMasterlistReference")]
        public async Task<ActionResult<List<ItemMasterlistReference>>> CreateItemMasterlistReference(ItemMasterlistReference itemMasterlistReference)
        {
            await ValidateMainPartsLinkAndOEM(itemMasterlistReference);

            var itemMasterlistReferenceList = await _itemMasterlistReferenceRepository.Create(itemMasterlistReference);
            return Ok(itemMasterlistReferenceList);
        }

        [HttpPut("UpdateItemMasterlistReference")]
        public async Task<ActionResult<List<ItemMasterlistReference>>> UpdateItemMasterlistReference(ItemMasterlistReference itemMasterlistReference)
        {
            await ValidateMainPartsLinkAndOEM(itemMasterlistReference);

            var itemMasterlistReferenceList = await _itemMasterlistReferenceRepository.Update(itemMasterlistReference);
            return Ok(itemMasterlistReferenceList);
        }

        [HttpDelete("DeleteItemMasterlistReference")]
        public async Task<ActionResult<List<ItemMasterlistReference>>> DeleteItemMasterlistReference(List<int> itemMasterlistReferenceIds)
        {
            var itemMasterlistReferenceList = await _itemMasterlistReferenceRepository.Delete(itemMasterlistReferenceIds);
            return Ok(itemMasterlistReferenceList);
        }

        [HttpPost("CreateItemMasterlistReferenceByProduct")]
        public async Task<ActionResult<List<ItemMasterlistReference>>> CreateItemMasterlistReferenceByProduct(ItemMasterlistReference itemMasterlistReference)
        {
            await ValidateMainPartsLinkAndOEM(itemMasterlistReference);

            var itemMasterlistReferenceList = await _itemMasterlistReferenceRepository.CreateByProduct(itemMasterlistReference);
            return Ok(itemMasterlistReferenceList);
        }

        [HttpPut("UpdateItemMasterlistReferenceByProduct")]
        public async Task<ActionResult<List<ItemMasterlistReference>>> UpdateItemMasterlistReferenceByProduct(ItemMasterlistReference itemMasterlistReference)
        {
            await ValidateMainPartsLinkAndOEM(itemMasterlistReference);

            var itemMasterlistReferenceList = await _itemMasterlistReferenceRepository.UpdateByProduct(itemMasterlistReference);
            return Ok(itemMasterlistReferenceList);
        }

        private async Task<bool> ValidateMainPartsLinkAndOEM(ItemMasterlistReference itemMasterlistReference)
        {
            // Check if Main Parts Link is checked
            if (itemMasterlistReference.IsMainPartsLink)
            {
                // Remove IsMainPartsLink on related record
                await _itemMasterlistReferenceRepository.RemoveExistingIsMainPartsLink(itemMasterlistReference.Id, itemMasterlistReference.ProductId);
            }

            // Check if Main OEM is checked
            if (itemMasterlistReference.IsMainOEM)
            {
                // Remove IsMainOEM on related record
                await _itemMasterlistReferenceRepository.RemoveExistingIsMainOEM(itemMasterlistReference.Id, itemMasterlistReference.ProductId);
            }
            return true;
        }
        #endregion
    }
}
