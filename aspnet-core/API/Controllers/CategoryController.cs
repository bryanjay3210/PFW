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
    public class CategoryController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(DataContext dataContext, ICategoryRepository categoryRepository)
        {
            _dataContext = dataContext;
            _categoryRepository = categoryRepository;
        }

        #region Get Data
        [HttpGet("GetCategories")]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            return Ok(await _categoryRepository.GetCategories());
        }

        [HttpGet("GetCategoryById")]
        public async Task<ActionResult<Category>> GetCategoryById(int categoryId)
        {
            var category = await _categoryRepository.GetCategory(categoryId);
            if (category == null)
                return NotFound("Category not found!");
            return Ok(category);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateCategory")]
        public async Task<ActionResult<List<Category>>> CreateCategory(Category category)
        {
            var categoryList = await _categoryRepository.Create(category);
            return Ok(categoryList);
        }

        [HttpPut("UpdateCategory")]
        public async Task<ActionResult<List<Category>>> UpdateCategory(Category category)
        {
            var categoryList = await _categoryRepository.Update(category);
            return Ok(categoryList);
        }

        [HttpDelete("DeleteCategory")]
        public async Task<ActionResult<List<Category>>> DeleteCategory(List<int> categoryIds)
        {
            var categoryList = await _categoryRepository.Delete(categoryIds);
            return Ok(categoryList);
        }
        #endregion
    }
}
