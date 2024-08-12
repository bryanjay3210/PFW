using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.DTO.Paginated;
using Domain.DomainModel.Interface;
using Infrastucture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IProductRepository _productRepository;
        private readonly IConfiguration _configuration;
        private readonly string _imageUrlBase;

        public ProductController(DataContext dataContext, IProductRepository productRepository, IConfiguration  configuration)
        {
            _dataContext = dataContext;
            _productRepository = productRepository;
            _configuration = configuration;
            _imageUrlBase = _configuration.GetValue<string>("imageUrlBase");
        }

        #region Get Data
        [HttpGet("GetProducts")]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            return Ok(await _productRepository.GetProducts());
        }

        [HttpGet("GetProductsFiltered")]
        public async Task<ActionResult<List<Product>>> GetProductsFiltered(ProductFilterDTO productFilterDTO)
        {
            return Ok(await _productRepository.GetProducts());
        }

        [HttpGet("GetProductsPaginated")]
        public async Task<ActionResult<PaginatedListDTO<Product>>> GetProductsPaginated(
            [FromQuery] bool isIncludeInactive = false,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "PartNumber",
            [FromQuery] string? sortOrder = "ASC",
            [FromQuery] string? search = ""
            )
        {
            var result = await _productRepository.GetProductsPaginated(isIncludeInactive, pageSize, pageIndex, sortColumn, sortOrder, search);
            result.Data.ForEach(e =>
            {
                e.ImageUrl = string.Format(_imageUrlBase, e.PartNumber.Trim());
            });
            return Ok(result);
        }

        [HttpGet("GetProductsList")]
        public async Task<ActionResult<List<ProductDTO>>> GetProductsList()
        {
            return Ok(await _productRepository.GetProductsList());
        }

        [HttpGet("GetSearchProductsListByYearMakeModelPaginated")]
        public async Task<ActionResult<PaginatedListDTO<ProductListDTO>>> GetSearchProductsListByYearMakeModelPaginated(
            [FromQuery] int state, // 1 = CA 2 = NV
            [FromQuery] int? year,
            [FromQuery] string? make,
            [FromQuery] string? model,
            [FromQuery] string? categoryIds,
            [FromQuery] string? sequenceIds,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "PartNumber",
            [FromQuery] string? sortOrder = "ASC",
            [FromQuery] string? search = "")
        {
            try
            {
                var productFilterDTO = new ProductFilterDTO()
                {
                    State = state,
                    Year = year,
                    Make = make,
                    Model = model,
                    CategoryIds = categoryIds != null ? categoryIds.Split(",").Select(int.Parse).ToList() : null,
                    SequenceIds = sequenceIds != null ? sequenceIds.Split(",").Select(int.Parse).ToList() : null
                };

                var result = await _productRepository.GetSearchProductsListByYearMakeModelPaginated(productFilterDTO, pageSize, pageIndex, sortColumn, sortOrder, search);
                result.Data.ForEach(e =>
                {
                    e.ImageUrl = string.Format(_imageUrlBase, e.PartNumber.Trim());
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }

        [HttpGet("GetSearchProductsListByPartNumberPaginated")]
        public async Task<ActionResult<PaginatedListDTO<ProductListDTO>>> GetSearchProductsListByPartNumberPaginated(
            [FromQuery] int state, // 1 = CA 2 = NV
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "PartNumber",
            [FromQuery] string? sortOrder = "ASC",
            [FromQuery] string? search = "")
        {
            try
            {
                return Ok(await _productRepository.GetSearchProductsListByPartNumberPaginated(state, pageSize, pageIndex, sortColumn, sortOrder, search));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("GetProductsListFilteredPaginated")]
        public async Task<ActionResult<PaginatedListDTO<ProductDTO>>> GetProductsListFilteredPaginated(
            [FromQuery] int? year,
            [FromQuery] string? make,
            [FromQuery] string? model,
            [FromQuery] string? categoryIds,
            [FromQuery] string? sequenceIds,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "PartNumber",
            [FromQuery] string? sortOrder = "ASC",
            [FromQuery] string? search = "")
        {
            try
            {
                var productFilterDTO = new ProductFilterDTO()
                {
                    Year = year,
                    Make = make,
                    Model = model,
                    CategoryIds = categoryIds != null ? categoryIds.Split(",").Select(int.Parse).ToList() : null,
                    SequenceIds = sequenceIds != null ? sequenceIds.Split(",").Select(int.Parse).ToList() : null

                };

                return Ok(await _productRepository.GetProductsListFilteredPaginated(productFilterDTO, pageSize, pageIndex, sortColumn, sortOrder, search));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("GetProductsListByPartNumberPaginated")]
        public async Task<ActionResult<PaginatedListDTO<ProductDTO>>> GetProductsListByPartNumberPaginated(
            [FromQuery] string? partNumber,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] string? sortColumn = "PartNumber",
            [FromQuery] string? sortOrder = "ASC",
            [FromQuery] string? search = "")
        {
            try
            {
                return Ok(await _productRepository.GetProductsListByPartNumberPaginated(partNumber, pageSize, pageIndex, sortColumn, sortOrder, search));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("GetProductById")]
        public async Task<ActionResult<Product>> GetProductById(int productId)
        {
            var product = await _productRepository.GetProductById(productId);
            if (product == null)
                return NotFound("Product not found!");
            return Ok(product);
        }

        [HttpGet("GetProductByIdAndState")]
        public async Task<ActionResult<Product>> GetProductByIdAndState(int productId, int state)
        {
            var product = await _productRepository.GetProductByIdAndState(productId, state);
            if (product == null)
                return NotFound("Product not found!");
            return Ok(product);
        }

        [HttpGet("GetProductByIdNoStocks")]
        public async Task<ActionResult<Product>> GetProductByIdNoStocks(int productId)
        {
            var product = await _productRepository.GetProductByIdNoStocks(productId);
            if (product == null)
                return NotFound("Product not found!");
            return Ok(product);
        }

        [HttpGet("GetProductByPartNumber")]
        public async Task<ActionResult<List<Product>>> GetProductByPartNumber(string partNumber)
        {
            var product = await _productRepository.GetProductByPartNumber(partNumber);
            return Ok(product);
        }

        [HttpGet("GetProductInLocationByPartNumber")]
        public async Task<ActionResult<List<Product>>> GetProductInLocationByPartNumber(int warehouseLocationId, string partNumber)
        {
            var product = await _productRepository.GetProductInLocationByPartNumber(warehouseLocationId, partNumber);
            return Ok(product);
        }

        [HttpGet("GetSingleProduct")]
        public async Task<ActionResult<Product>> GetSingleProduct(string searchKey)
        {
            var product = await _productRepository.GetSingleProduct(searchKey);
            return Ok(product);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateProduct")]
        public async Task<ActionResult<List<Product>>> CreateProduct(Product product)
        {
            var productList = await _productRepository.Create(product);
            return Ok(productList);
        }

        [HttpPut("UpdateProduct")]
        public async Task<ActionResult<List<Product>>> UpdateProduct(Product product)
        {
            var productList = await _productRepository.Update(product);
            return Ok(productList);
        }

        [HttpDelete("DeleteProduct")]
        public async Task<ActionResult<List<Product>>> DeleteProduct(List<int> productIds)
        {
            var productList = await _productRepository.Delete(productIds);
            return Ok(productList);
        }
        #endregion
    }
}
