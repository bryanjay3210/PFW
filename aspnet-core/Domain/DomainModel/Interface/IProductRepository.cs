using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.DTO.Paginated;

namespace Domain.DomainModel.Interface
{
    public interface IProductRepository : IRepository<Product>
    {
        #region Get Data
        Task<ProductDTO> GetProductById(int productId);
        Task<ProductDTO> GetProductByIdAndState(int productId, int state);
        Task<ProductDTO> GetProductByIdNoStocks(int productId);
        Task<List<Product>?> GetProductByPartNumber(string partNumber);
        Task<List<Product>?> GetProductInLocationByPartNumber(int warehouseLocationId, string partNumber);
        Task<List<Product>> GetProducts();
        Task<PaginatedListDTO<Product>> GetProductsPaginated(bool isIncludeInactive, int pageSize, int pageIndex, string? sortColumn = "PartNumber", string? sortOrder = "ASC", string? search = "");
        Task<List<ProductDTO>> GetProductsList();
        Task<PaginatedListDTO<ProductListDTO>> GetSearchProductsListByYearMakeModelPaginated(ProductFilterDTO productFilterDTO, int pageSize, int pageIndex, string? sortColumn = "PartNumber", string? sortOrder = "ASC", string? search = "");
        Task<PaginatedListDTO<ProductListDTO>> GetSearchProductsListByPartNumberPaginated(int state, int pageSize, int pageIndex, string? sortColumn = "PartNumber", string? sortOrder = "ASC", string? search = "");
        Task<PaginatedListDTO<ProductDTO>> GetProductsListFilteredPaginated(ProductFilterDTO productFilterDTO, int pageSize, int pageIndex, string? sortColumn = "PartNumber", string? sortOrder = "ASC", string? search = "");
        Task<PaginatedListDTO<ProductDTO>> GetProductsListByPartNumberPaginated(string? partNumber, int pageSize, int pageIndex, string? sortColumn = "PartNumber", string? sortOrder = "ASC", string? search = "");
        Task<Product> GetSingleProduct(string searchKey);
        #endregion

        #region Save Data
        Task<List<Product>> Create(Product product);
        Task<List<Product>> Update(Product product);
        Task<List<Product>> Delete(List<int> productIds);
        Task<List<Product>> SoftDelete(List<int> productIds);
        #endregion
    }
}
