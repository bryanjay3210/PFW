using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.Lookup;
using Domain.DomainModel.Entity.RolesAndAccess;
using Domain.DomainModel.Interface;
using Infrastucture;
using Infrastucture.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        private readonly ILookupRepository _lookupRepository;
        private readonly IWarehouseRepository _warehouseRepository;

        public LookupController(ILookupRepository lookupRepository, IWarehouseRepository warehouseRepository)
        {
            _lookupRepository = lookupRepository ?? throw new ArgumentNullException(nameof(lookupRepository));
            _warehouseRepository = warehouseRepository ?? throw new ArgumentNullException(nameof(warehouseRepository));
        }

        #region Get Data
        [HttpGet("GetWarehousePartsByProductId")]
        public async Task<ActionResult<List<WarehousePartDTO>>> GetWarehousePartsByProductId(int productId)
        {
            var warehousePartList = await _warehouseRepository.GetWarehousePartsByProductId(productId);
            if (warehousePartList == null)
                return NotFound("Warehouse not found!");
            return Ok(warehousePartList);
        }

        // CustomerType
        [HttpGet("GetCustomerTypes")]
        public async Task<ActionResult<List<CustomerType>>> GetCustomerTypes()
        {
            var result = await _lookupRepository.GetLookupData<CustomerType>(ct => ct.IsActive == true && ct.IsDeleted == false);
            if (result == null)
                return NotFound("Customer Types not found.");

            return Ok(result);
        }

        [HttpGet("GetCustomerTypeById")]
        public async Task<ActionResult<CustomerType>> GetCustomerTypeById(int customerTypeId)
        {
            var result = await _lookupRepository.GetLookupDataById<CustomerType>(ct => ct.Id == customerTypeId && ct.IsActive == true && ct.IsDeleted == false);
            
            if (result == null)
                return NotFound("Customer Type not found.");

            return Ok(result);
        }

        // LocationType
        [HttpGet("GetLocationTypes")]
        public async Task<ActionResult<List<LocationType>>> GetLocationTypes()
        {
            var result = await _lookupRepository.GetLookupData<LocationType>(lt => lt.IsActive == true && lt.IsDeleted == false);
            if (result == null)
                return NotFound("Location Types not found!");

            return Ok(result);
        }

        [HttpGet("GetLocationTypeById")]
        public async Task<ActionResult<LocationType>> GetLocationTypeById(int locationTypeId)
        {
            var result = await _lookupRepository.GetLookupDataById<LocationType>(lt => lt.Id == locationTypeId && lt.IsActive == true && lt.IsDeleted == false);

            if (result == null)
                return NotFound("Location Type not found.");

            return Ok(result);
        }

        [HttpGet("GetPositionTypes")]
        public async Task<ActionResult<List<PositionType>>> GetPositionTypes()
        {
            var result = await _lookupRepository.GetLookupData<PositionType>(pt => pt.IsActive == true && pt.IsDeleted == false);
            if (result == null)
                return NotFound("Position Types not found!");

            return Ok(result);
        }

        [HttpGet("GetPaymentTypes")]
        public async Task<ActionResult<List<PaymentType>>> GetPaymentTypes()
        {
            var result = await _lookupRepository.GetLookupData<PaymentType>(pt => pt.IsActive == true && pt.IsDeleted == false);
            if (result == null)
                return NotFound("Payment Types not found!");

            return Ok(result);
        }

        [HttpGet("GetPositionTypeById")]
        public async Task<ActionResult<PositionType>> GetPositionTypeById(int positionTypeId)
        {
            var result = await _lookupRepository.GetLookupDataById<PositionType>(pt => pt.Id == positionTypeId && pt.IsActive == true && pt.IsDeleted == false);

            if (result == null)
                return NotFound("Position Type not found.");

            return Ok(result);
        }

        // UserType
        [HttpGet("GetUserTypes")]
        public async Task<ActionResult<List<UserType>>> GetUserTypes()
        {
            var result = await _lookupRepository.GetLookupData<UserType>(ct => ct.IsActive == true && ct.IsDeleted == false);
            if (result == null)
                return NotFound("User Types not found.");

            return Ok(result);
        }

        [HttpGet("GetUserTypeById")]
        public async Task<ActionResult<UserType>> GetUserTypeById(int userTypeId)
        {
            var result = await _lookupRepository.GetLookupDataById<UserType>(ct => ct.Id == userTypeId && ct.IsActive == true && ct.IsDeleted == false);

            if (result == null)
                return NotFound("User Type not found.");

            return Ok(result);
        }

        // AccessType
        [HttpGet("GetAccessTypes")]
        public async Task<ActionResult<List<AccessType>>> GetAccessTypes()
        {
            var result = await _lookupRepository.GetLookupData<AccessType>(ct => ct.IsActive == true && ct.IsDeleted == false);
            if (result == null)
                return NotFound("Access Types not found.");

            return Ok(result);
        }

        [HttpGet("GetAccessTypeById")]
        public async Task<ActionResult<AccessType>> GetAccessTypeById(int accessTypeId)
        {
            var result = await _lookupRepository.GetLookupDataById<AccessType>(ct => ct.Id == accessTypeId && ct.IsActive == true && ct.IsDeleted == false);

            if (result == null)
                return NotFound("Access Type not found.");

            return Ok(result);
        }

        // PriceLevel
        [HttpGet("GetPriceLevels")]
        public async Task<ActionResult<List<PriceLevel>>> GetPriceLevels()
        {
            var result = await _lookupRepository.GetLookupData<PriceLevel>(ct => ct.IsActive == true && ct.IsDeleted == false);
            if (result == null)
                return NotFound("Price Levels not found.");

            return Ok(result);
        }

        //[HttpGet("GetPriceLevelById")]
        //public async Task<ActionResult<PriceLevel>> GetPriceLevelById(int priceLevelId)
        //{
        //    var result = await _lookupRepository.GetLookupDataById<PriceLevel>(ct => ct.Id == priceLevelId && ct.IsActive == true && ct.IsDeleted == false);

        //    if (result == null)
        //        return NotFound("Price Level not found.");

        //    return Ok(result);
        //}

        // PaymentTerm
        [HttpGet("GetPaymentTerms")]
        public async Task<ActionResult<List<PaymentTerm>>> GetPaymentTerms()
        {
            var result = await _lookupRepository.GetLookupData<PaymentTerm>(ct => ct.IsActive == true && ct.IsDeleted == false);
            if (result == null)
                return NotFound("Payment Terms not found.");

            return Ok(result);
        }

        // SalesRepresentative
        [HttpGet("GetSalesRepresentatives")]
        public async Task<ActionResult<List<SalesRepresentative>>> GetSalesRepresentatives()
        {
            var result = await _lookupRepository.GetLookupData<SalesRepresentative>(ct => ct.IsActive == true && ct.IsDeleted == false);
            if (result == null)
                return NotFound("Sales Representatives not found.");

            return Ok(result);
        }

        // Warehouse
        [HttpGet("GetWarehouses")]
        public async Task<ActionResult<List<Warehouse>>> GetWarehouses()
        {
            var result = await _lookupRepository.GetLookupData<Warehouse>(ct => ct.IsActive == true && ct.IsDeleted == false);
            if (result == null)
                return NotFound("Warehouses not found.");

            return Ok(result);
        }

        // Year List
        [HttpGet("GetYearsListDistinct")]
        public async Task<ActionResult<List<YearDTO>>> GetYearsListDistinct()
        {
            var result = await _lookupRepository.GetYearsListDistinct();
            if (result == null)
                return NotFound("Makes not found.");

            return Ok(result);
        }

        // Make List
        [HttpGet("GetMakesListDistinct")]
        public async Task<ActionResult<List<MakeDTO>>> GetMakesListDistinct()
        {
            var result = await _lookupRepository.GetMakesListDistinct();
            if (result == null)
                return NotFound("Makes not found.");

            return Ok(result);
        }

        // Model List
        [HttpGet("GetModelsListDistinct")]
        public async Task<ActionResult<List<ModelDTO>>> GetModelsListDistinct()
        {
            var result = await _lookupRepository.GetModelsListDistinct();
            if (result == null)
                return NotFound("Models not found.");

            return Ok(result);
        }

        // Category List
        [HttpGet("GetCategoriesListDistinct")]
        public async Task<ActionResult<List<CategoryDTO>>> GetCategoriesListDistinct()
        {
            var result = await _lookupRepository.GetCategoriesListDistinct();
            if (result == null)
                return NotFound("Categories not found.");

            return Ok(result);
        }

        // Sequence List
        [HttpGet("GetSequencesListDistinct")]
        public async Task<ActionResult<List<SequenceDTO>>> GetSequencesListDistinct()
        {
            var result = await _lookupRepository.GetSequencesListDistinct();
            if (result == null)
                return NotFound("Squences not found.");

            return Ok(result);
        }

        //-----------------------------------------------------------------
        // Make List By Year
        [HttpGet("GetMakesListByYear")]
        public async Task<ActionResult<List<MakeDTO>>> GetMakesListByYear(
            [FromQuery] int? year,
            [FromQuery] string? make,
            [FromQuery] string? model,
            [FromQuery] string? categoryIds,
            [FromQuery] string? sequenceIds
            )
        {
            var productFilterDTO = new ProductFilterDTO()
            {
                Year = year,
                Make = make,
                Model = model,
                CategoryIds = categoryIds != null ? categoryIds.Split(",").Select(int.Parse).ToList() : null,
                SequenceIds = sequenceIds != null ? sequenceIds.Split(",").Select(int.Parse).ToList() : null

            };

            var result = await _lookupRepository.GetMakesListByYear(productFilterDTO);
            //if (result == null)
            //    return NotFound("Makes not found.");

            return Ok(result);
        }

        // Model List By Make
        [HttpGet("GetModelsListByMake")]
        public async Task<ActionResult<List<ModelDTO>>> GetModelsListByMake(
            [FromQuery] int? year,
            [FromQuery] string? make,
            [FromQuery] string? model,
            [FromQuery] string? categoryIds,
            [FromQuery] string? sequenceIds
            )
        {
            var productFilterDTO = new ProductFilterDTO()
            {
                Year = year,
                Make = make,
                Model = model,
                CategoryIds = categoryIds != null ? categoryIds.Split(",").Select(int.Parse).ToList() : null,
                SequenceIds = sequenceIds != null ? sequenceIds.Split(",").Select(int.Parse).ToList() : null

            };

            var result = await _lookupRepository.GetModelsListByMake(productFilterDTO);
            //if (result == null)
            //    return NotFound("Models not found.");

            return Ok(result);
        }

        // Category List By Model
        [HttpGet("GetCategoriesListByModel")]
        public async Task<ActionResult<List<CategoryDTO>>> GetCategoriesListByModel(
            [FromQuery] int? year,
            [FromQuery] string? make,
            [FromQuery] string? model,
            [FromQuery] string? categoryIds,
            [FromQuery] string? sequenceIds
            )
        {
            var productFilterDTO = new ProductFilterDTO()
            {
                Year = year,
                Make = make,
                Model = model,
                CategoryIds = categoryIds != null ? categoryIds.Split(",").Select(int.Parse).ToList() : null,
                SequenceIds = sequenceIds != null ? sequenceIds.Split(",").Select(int.Parse).ToList() : null

            };

            var result = await _lookupRepository.GetCategoriesListByModel(productFilterDTO);
            //if (result == null)
            //    return NotFound("Categories not found.");

            return Ok(result);
        }

        // Sequence List By Category
        [HttpGet("GetSequencesListByCategoryId")]
        public async Task<ActionResult<List<SequenceDTO>>> GetSequencesListByCategoryId(
            [FromQuery] int? year,
            [FromQuery] string? make,
            [FromQuery] string? model,
            [FromQuery] string? categoryIds,
            [FromQuery] string? sequenceIds
            )
        {
            var productFilterDTO = new ProductFilterDTO()
            {
                Year = year,
                Make = make,
                Model = model,
                CategoryIds = categoryIds != null ? categoryIds.Split(",").Select(int.Parse).ToList() : null,
                SequenceIds = sequenceIds != null ? sequenceIds.Split(",").Select(int.Parse).ToList() : null

            };

            var result = await _lookupRepository.GetSequencesListByCategoryId(productFilterDTO);
            //if (result == null)
            //    return NotFound("Squences not found.");

            return Ok(result);
        }
        #endregion
    }
}
