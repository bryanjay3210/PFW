using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.DTO.Paginated;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Security.Cryptography;

namespace Infrastucture.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public ProductRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<ProductDTO> GetProductById(int productId)
        {
            var result = new ProductDTO();
            var product = await _context.Products.Where(e => e.Id == productId).FirstOrDefaultAsync();

            if (product != null)
            {
                var productDTO = new ProductDTO()
                {
                    Brand = product.Brand,
                    CategoryId = product.CategoryId,
                    Id = product.Id,
                    ListPrice = product.PriceLevel1,
                    PartDescription = product.PartDescription != null ? product.PartDescription.Trim() : "",
                    PartNumber = product.PartNumber.Trim(),
                    PartSizeId = product.PartSizeId,
                    PriceLevel1 = product.PriceLevel1,
                    PriceLevel2 = product.PriceLevel2,
                    PriceLevel3 = product.PriceLevel3,
                    PriceLevel4 = product.PriceLevel4,
                    PriceLevel5 = product.PriceLevel5,
                    PriceLevel6 = product.PriceLevel6,
                    PriceLevel7 = product.PriceLevel7,
                    PriceLevel8 = product.PriceLevel8,
                    SequenceId = product.SequenceId,
                    StatusId = product.StatusId,
                    CurrentCost = product.CurrentCost.HasValue ? product.CurrentCost.Value : 0
                };


                var partsList = await _context.ItemMasterlistReferences.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber).ToListAsync();

                var catalogList = await _context.PartsCatalogs.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber && e.ProductId == product.Id).ToListAsync();
                var partsLinkNumber = string.Empty;
                var OEMNumber = string.Empty;

                if (partsList != null && partsList.Any())
                {
                    var partsLink = partsList.Where(e => e.IsMainPartsLink).FirstOrDefault();
                    partsLinkNumber = partsLink != null ? partsLink.PartsLinkNumber : string.Empty;

                    partsLink = partsList.Where(e => e.IsMainOEM).FirstOrDefault();
                    OEMNumber = partsLink != null ? partsLink.OEMNumber : string.Empty;
                }

                var vendorList = new List<VendorCatalog>();

                if (!string.IsNullOrEmpty(partsLinkNumber))
                {
                    vendorList = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim().ToLower() == partsLinkNumber.Trim().ToLower()).ToListAsync();
                }

                //Get Warehouse Stocks with Location Name
                var stocks = await (
                    from stock in _context.WarehouseStocks
                    join location in _context.WarehouseLocations on stock.WarehouseLocationId equals location.Id
                    where stock.ProductId == product.Id //&& stock.Quantity > 0
                    select new WarehouseStock
                    {
                        Id = stock.Id,
                        CreatedBy = stock.CreatedBy,
                        CreatedDate = stock.CreatedDate,
                        IsActive = stock.IsActive,
                        IsDeleted = stock.IsDeleted,
                        Location = location.Location,
                        ModifiedBy = stock.ModifiedBy,
                        ModifiedDate = stock.ModifiedDate,
                        ProductId = product.Id,
                        Quantity = stock.Quantity > 0 ? stock.Quantity : 0,
                        WarehouseId = stock.WarehouseId,
                        WarehouseLocationId = stock.WarehouseLocationId,
                        CurrentCost = product.CurrentCost.HasValue ? product.CurrentCost.Value : 0
                    }).ToListAsync();

                // Get Stocks Quantity
                // var stocks = await _context.WarehouseStocks.Where(e => e.ProductId == product.Id).ToListAsync();
                int stockCount = (stocks != null && stocks.Count > 0) ? stocks.Sum(s => s.Quantity) : 0;

                productDTO.PartsLinkNumber = partsLinkNumber;
                productDTO.OEMNumber = OEMNumber;
                productDTO.PartsLinks = (partsList != null && partsList.Any()) ? string.Join(',', partsList.Select(e => e.PartsLinkNumber)) : "";
                productDTO.OEMs = (partsList != null && partsList.Any()) ? string.Join(',', partsList.Select(e => e.OEMNumber)) : "";
                productDTO.VendorCodes = (vendorList != null && vendorList.Any()) ? string.Join(',', vendorList.Select(e => e.VendorCode)) : "";
                productDTO.YearFrom = (catalogList != null && catalogList.Any()) ? catalogList.Min(e => e.YearFrom) : 1900;
                productDTO.YearTo = (catalogList != null && catalogList.Any()) ? catalogList.Max(e => e.YearTo) : 1900;
                productDTO.VendorCatalogs = vendorList;
                productDTO.Stock = stockCount;
                productDTO.Stocks = stocks;
                result = productDTO;
            }

            return result;
        }

        public async Task<ProductDTO> GetProductByIdAndState(int productId, int state)
        {
            var stockSettings = await _context.StockSettings.FirstOrDefaultAsync();
            if (stockSettings == null) return new ProductDTO();

            var result = new ProductDTO();
            var product = await _context.Products.Where(e => e.Id == productId).FirstOrDefaultAsync();

            if (product != null)
            {
                var productDTO = new ProductDTO()
                {
                    Brand = product.Brand,
                    CategoryId = product.CategoryId,
                    Id = product.Id,
                    ListPrice = product.PriceLevel1,
                    PartDescription = product.PartDescription != null ? product.PartDescription.Trim() : "",
                    PartNumber = product.PartNumber.Trim(),
                    PartSizeId = product.PartSizeId,
                    PriceLevel1 = product.PriceLevel1,
                    PriceLevel2 = product.PriceLevel2,
                    PriceLevel3 = product.PriceLevel3,
                    PriceLevel4 = product.PriceLevel4,
                    PriceLevel5 = product.PriceLevel5,
                    PriceLevel6 = product.PriceLevel6,
                    PriceLevel7 = product.PriceLevel7,
                    PriceLevel8 = product.PriceLevel8,
                    SequenceId = product.SequenceId,
                    StatusId = product.StatusId,
                    CurrentCost = product.CurrentCost.HasValue ? product.CurrentCost.Value : 0
                };


                var partsList = await _context.ItemMasterlistReferences.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber).ToListAsync();

                var catalogList = await _context.PartsCatalogs.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber && e.ProductId == product.Id).ToListAsync();
                var partsLinkNumber = string.Empty;
                var OEMNumber = string.Empty;

                if (partsList != null && partsList.Any())
                {
                    var partsLink = partsList.Where(e => e.IsMainPartsLink).FirstOrDefault();
                    partsLinkNumber = partsLink != null ? partsLink.PartsLinkNumber : string.Empty;

                    partsLink = partsList.Where(e => e.IsMainOEM).FirstOrDefault();
                    OEMNumber = partsLink != null ? partsLink.OEMNumber : string.Empty;
                }

                var vendorList = new List<VendorCatalog>();

                if (!string.IsNullOrEmpty(partsLinkNumber))
                {
                    vendorList = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim().ToLower() == partsLinkNumber.Trim().ToLower()).ToListAsync();
                }

                //Get Warehouse Stocks with Location Name
                var stocks = await (
                    from stock in _context.WarehouseStocks
                    join location in _context.WarehouseLocations on stock.WarehouseLocationId equals location.Id
                    where stock.ProductId == product.Id && //&& stock.Quantity > 0 
                    (state == 1 ? (stockSettings.IsCaliforniaCAProduct ? stockSettings.IsCaliforniaNVProduct ? true : stock.WarehouseId == 1 : stockSettings.IsCaliforniaNVProduct ? stock.WarehouseId == 2 : false) :
                                  (stockSettings.IsNevadaCAProduct ? stockSettings.IsNevadaNVProduct ? true : stock.WarehouseId == 1 : stockSettings.IsNevadaNVProduct ? stock.WarehouseId == 2 : false))
                    select new WarehouseStock
                    {
                        Id = stock.Id,
                        CreatedBy = stock.CreatedBy,
                        CreatedDate = stock.CreatedDate,
                        IsActive = stock.IsActive,
                        IsDeleted = stock.IsDeleted,
                        Location = location.Location,
                        ModifiedBy = stock.ModifiedBy,
                        ModifiedDate = stock.ModifiedDate,
                        ProductId = product.Id,
                        Quantity = stock.Quantity > 0 ? stock.Quantity : 0,
                        WarehouseId = stock.WarehouseId,
                        WarehouseLocationId = stock.WarehouseLocationId,
                        CurrentCost = product.CurrentCost != null ? product.CurrentCost.Value : 0
                    }).ToListAsync();

                // Get Stocks Quantity
                // var stocks = await _context.WarehouseStocks.Where(e => e.ProductId == product.Id).ToListAsync();
                int stockCount = (stocks != null && stocks.Count > 0) ? stocks.Sum(s => s.Quantity) : 0;

                productDTO.PartsLinkNumber = partsLinkNumber;
                productDTO.OEMNumber = OEMNumber;
                productDTO.PartsLinks = (partsList != null && partsList.Any()) ? string.Join(',', partsList.Select(e => e.PartsLinkNumber)) : "";
                productDTO.OEMs = (partsList != null && partsList.Any()) ? string.Join(',', partsList.Select(e => e.OEMNumber)) : "";
                productDTO.VendorCodes = (vendorList != null && vendorList.Any()) ? string.Join(',', vendorList.Select(e => e.VendorCode)) : "";
                productDTO.YearFrom = (catalogList != null && catalogList.Any()) ? catalogList.Min(e => e.YearFrom) : 1900;
                productDTO.YearTo = (catalogList != null && catalogList.Any()) ? catalogList.Max(e => e.YearTo) : 1900;
                productDTO.VendorCatalogs = vendorList;
                productDTO.Stock = stockCount;
                productDTO.Stocks = stocks;
                result = productDTO;
            }

            return result;
        }

        public async Task<ProductDTO> GetProductByIdNoStocks(int productId)
        {
            var result = new ProductDTO();
            var product = await _context.Products.Where(e => e.Id == productId).FirstOrDefaultAsync();
            
            if (product != null)
            {
                var productDTO = new ProductDTO()
                {
                    Brand = product.Brand,
                    CategoryId = product.CategoryId,
                    Id = product.Id,
                    ListPrice = product.PriceLevel1,
                    PartDescription = product.PartDescription != null ? product.PartDescription.Trim() : "",
                    PartNumber = product.PartNumber.Trim(),
                    PartSizeId = product.PartSizeId,
                    PriceLevel1 = product.PriceLevel1,
                    PriceLevel2 = product.PriceLevel2,
                    PriceLevel3 = product.PriceLevel3,
                    PriceLevel4 = product.PriceLevel4,
                    PriceLevel5 = product.PriceLevel5,
                    PriceLevel6 = product.PriceLevel6,
                    PriceLevel7 = product.PriceLevel7,
                    PriceLevel8 = product.PriceLevel8,
                    SequenceId = product.SequenceId,
                    StatusId = product.StatusId,
                    CurrentCost = product.CurrentCost.HasValue ? product.CurrentCost.Value : 0
                };

                
                var partsList = await _context.ItemMasterlistReferences.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber).ToListAsync();

                var catalogList = await _context.PartsCatalogs.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber && e.ProductId == product.Id).ToListAsync();
                var partsLinkNumber = string.Empty;
                var OEMNumber = string.Empty;

                if (partsList != null && partsList.Any())
                {
                    var partsLink = partsList.Where(e => e.IsMainPartsLink).FirstOrDefault();
                    partsLinkNumber = partsLink != null ? partsLink.PartsLinkNumber : string.Empty;

                    partsLink = partsList.Where(e => e.IsMainOEM).FirstOrDefault();
                    OEMNumber = partsLink != null ? partsLink.OEMNumber : string.Empty;
                }

                var vendorList = new List<VendorCatalog>();

                if (!string.IsNullOrEmpty(partsLinkNumber))
                {
                    vendorList = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim().ToLower() == partsLinkNumber.Trim().ToLower()).ToListAsync();
                }

                //Get Warehouse Stocks with Location Name
                //var stocks = (
                //    from stock in _context.WarehouseStocks
                //    join location in _context.WarehouseLocations on stock.WarehouseLocationId equals location.Id
                //    where stock.ProductId == product.Id
                //    select new WarehouseStock
                //    {
                //        Id = stock.Id,
                //        CreatedBy = stock.CreatedBy,
                //        CreatedDate = stock.CreatedDate,
                //        IsActive = stock.IsActive,
                //        IsDeleted = stock.IsDeleted,
                //        Location = location.Location,
                //        ModifiedBy = stock.ModifiedBy,
                //        ModifiedDate = stock.ModifiedDate,
                //        ProductId = product.Id,
                //        Quantity = stock.Quantity,
                //        WarehouseId = stock.WarehouseId,
                //        WarehouseLocationId = stock.WarehouseLocationId
                //    }).ToList();

                // Get Stocks Quantity
                // var stocks = await _context.WarehouseStocks.Where(e => e.ProductId == product.Id).ToListAsync();
                int stockCount = 0; // (stocks != null && stocks.Count > 0) ? stocks.Sum(s => s.Quantity) : 0;

                productDTO.PartsLinkNumber = partsLinkNumber;
                productDTO.OEMNumber = OEMNumber;
                productDTO.PartsLinks = (partsList != null && partsList.Any()) ? string.Join(',', partsList.Select(e => e.PartsLinkNumber)) : "";
                productDTO.OEMs = (partsList != null && partsList.Any()) ? string.Join(',', partsList.Select(e => e.OEMNumber)) : "";
                productDTO.VendorCodes = (vendorList != null && vendorList.Any()) ? string.Join(',', vendorList.Select(e => e.VendorCode)) : "";
                productDTO.YearFrom = (catalogList != null && catalogList.Any()) ? catalogList.Min(e => e.YearFrom) : 1900;
                productDTO.YearTo = (catalogList != null && catalogList.Any()) ? catalogList.Max(e => e.YearTo) : 1900;
                productDTO.VendorCatalogs = vendorList;
                productDTO.Stock = stockCount;
                productDTO.Stocks = new List<WarehouseStock>(); // stocks;
                result = productDTO;
            }

            return result;
        }

        public async Task<List<Product>?> GetProductByPartNumber(string partNumber)
        {
            partNumber = partNumber.Trim().ToLower();
            var result = new List<Product>();
            
            result = await (
                from p in _context.Products
                join imr in _context.ItemMasterlistReferences on p.Id equals imr.ProductId
                from vc in _context.VendorCatalogs
                    .Where(e => e.IsActive && !e.IsDeleted && imr.PartsLinkNumber.Trim().ToLower() == e.PartsLinkNumber.Trim().ToLower())
                    .DefaultIfEmpty()
                where (p.IsActive && !p.IsDeleted && imr.IsActive && !imr.IsDeleted &&
                       (p.PartNumber.Trim().ToLower() == partNumber || imr.PartsLinkNumber.Trim().ToLower() == partNumber || imr.OEMNumber.Trim().ToLower() == partNumber || vc.VendorPartNumber.Trim().ToLower() == partNumber))
                select p)
                .Distinct()
                .ToListAsync();

            return result;
        }

        public async Task<List<Product>?> GetProductInLocationByPartNumber(int warehouseLocationId, string partNumber)
        {
            var result = new List<Product>();
            partNumber = partNumber.Trim().ToLower();
            
            result = await (
                from wl in _context.WarehouseLocations where wl.Id == warehouseLocationId
                join ws in _context.WarehouseStocks on wl.Id equals ws.WarehouseLocationId
                join p in _context.Products on ws.ProductId equals p.Id
                join imr in _context.ItemMasterlistReferences on p.Id equals imr.ProductId
                from vc in _context.VendorCatalogs
                    .Where(e => e.IsActive && !e.IsDeleted && imr.PartsLinkNumber.Trim().ToLower() == e.PartsLinkNumber.Trim().ToLower())
                    .DefaultIfEmpty()
                where (p.IsActive && !p.IsDeleted && imr.IsActive && !imr.IsDeleted &&
                       (p.PartNumber.Trim().ToLower() == partNumber || imr.PartsLinkNumber.Trim().ToLower() == partNumber || vc.VendorPartNumber.Trim().ToLower() == partNumber))
                select p)
                .Distinct()
                .ToListAsync();

            if (result.Count > 0 )
            {
                foreach ( var item in result )
                {
                    var stock = await _context.WarehouseStocks.FirstOrDefaultAsync(e => e.WarehouseLocationId == warehouseLocationId && e.ProductId == item.Id);
                    if (stock != null)
                    {
                        item.StockQuantity = stock.Quantity;
                    }
                }
            }

            return result;
        }

        public async Task<List<Product>> GetProducts()
        {
            var products = await _context.Products
                .OrderBy(e => e.PartNumber)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
            
            foreach (var product in products)
            {
                int minYear = _context.PartsCatalogs.Where(e => e.ProductId == product.Id && e.IsActive && !e.IsDeleted).Min(m => (int?)m.YearFrom) ?? 0;
                int maxYear = _context.PartsCatalogs.Where(e => e.ProductId == product.Id && e.IsActive && !e.IsDeleted).Max(m => (int?)m.YearTo) ?? 0;

                var partsList = await _context.ItemMasterlistReferences.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber).ToListAsync();
                var vendorList = await _context.VendorCatalogs.Where(e => e.IsActive && !e.IsDeleted && partsList.Select(e => e.PartsLinkNumber).ToList().Contains(e.PartsLinkNumber)).ToListAsync();
                
                product.PartsLinkList = partsList;
                product.VendorCatalogList = vendorList;
                product.YearFrom = minYear;
                product.YearTo = maxYear;

                product.OriginalPartNumber = product.PartNumber;
            }

            return products;
        }

        public async Task<PaginatedListDTO<Product>> GetProductsPaginated(bool isIncludeInactive, int pageSize, int pageIndex, string? sortColumn = "PartNumber", string? sortOrder = "ASC", string? search = "")
        {
            var products = new List<Product>();
            var recordCount = (
                from p in _context.Products
                join imr in _context.ItemMasterlistReferences
                on p.Id equals imr.ProductId
                from vc in _context.VendorCatalogs
                    .Where(e => e.IsActive && !e.IsDeleted && imr.PartsLinkNumber.Trim().ToLower() == e.PartsLinkNumber.Trim().ToLower())
                    .DefaultIfEmpty()
                where (isIncludeInactive ? true : p.IsActive) && !p.IsDeleted &&
                      ((string.IsNullOrEmpty(search) ? true : p.PartDescription.Trim().ToLower().Contains(search.ToLower())) || 
                       (string.IsNullOrEmpty(search) ? true : p.PartNumber.Trim().ToLower().Contains(search.ToLower())) ||
                       (string.IsNullOrEmpty(search) ? true : imr.PartsLinkNumber.Trim().ToLower().Contains(search.ToLower())) ||
                       (string.IsNullOrEmpty(search) ? true : imr.OEMNumber.Trim().ToLower().Contains(search.ToLower())) ||
                       (string.IsNullOrEmpty(search) ? true : string.IsNullOrEmpty(vc.VendorPartNumber) ? false : vc.VendorPartNumber.Trim().ToLower().Contains(search.ToLower())))
                select p)
                .Distinct()
                .Count();

            if (sortOrder == "ASC")
            {
                products = await (
                from p in _context.Products
                join imr in _context.ItemMasterlistReferences on p.Id equals imr.ProductId
                from vc in _context.VendorCatalogs
                    .Where(e => e.IsActive && !e.IsDeleted && imr.PartsLinkNumber.Trim().ToLower() == e.PartsLinkNumber.Trim().ToLower())
                    .DefaultIfEmpty()
                where (isIncludeInactive ? true : p.IsActive) && !p.IsDeleted && 
                      ((string.IsNullOrEmpty(search) ? true : p.PartDescription.Trim().ToLower().Contains(search.ToLower())) ||
                       (string.IsNullOrEmpty(search) ? true : p.PartNumber.Trim().ToLower().Contains(search.ToLower())) ||
                       (string.IsNullOrEmpty(search) ? true : imr.PartsLinkNumber.Trim().ToLower().Contains(search.ToLower())) ||
                       (string.IsNullOrEmpty(search) ? true : imr.OEMNumber.Trim().ToLower().Contains(search.ToLower())) ||
                       (string.IsNullOrEmpty(search) ? true : string.IsNullOrEmpty(vc.VendorPartNumber) ? false : vc.VendorPartNumber.Trim().ToLower().Contains(search.ToLower())))
                select p)
                .Distinct()
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();
                //.OrderBy(e => "e." + sortColumn)
                //.OrderBy(e => e.PartNumber)
            }
            else
            {
                products = await (
                from p in _context.Products
                join imr in _context.ItemMasterlistReferences on p.Id equals imr.ProductId
                from vc in _context.VendorCatalogs
                    .Where(e => e.IsActive && !e.IsDeleted && imr.PartsLinkNumber.Trim().ToLower() == e.PartsLinkNumber.Trim().ToLower())
                    .DefaultIfEmpty()
                where (isIncludeInactive ? true : p.IsActive) && !p.IsDeleted &&
                      ((string.IsNullOrEmpty(search) ? true : p.PartNumber.Trim().ToLower().Contains(search.ToLower())) ||
                       (string.IsNullOrEmpty(search) ? true : imr.PartsLinkNumber.Trim().ToLower().Contains(search.ToLower())) ||
                       (string.IsNullOrEmpty(search) ? true : imr.OEMNumber.Trim().ToLower().Contains(search.ToLower())) ||
                       (string.IsNullOrEmpty(search) ? true : string.IsNullOrEmpty(vc.VendorPartNumber) ? false : vc.VendorPartNumber.Trim().ToLower().Contains(search.ToLower())))
                select p)
                .Distinct()
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();
                //.OrderByDescending(e => "e." + sortColumn)
                //.OrderByDescending(e => e.PartNumber)
            }

            foreach (var product in products)
            {
                int minYear = _context.PartsCatalogs.Where(e => e.ProductId == product.Id && e.IsActive && !e.IsDeleted).Min(m => (int?)m.YearFrom) ?? 0;
                int maxYear = _context.PartsCatalogs.Where(e => e.ProductId == product.Id && e.IsActive && !e.IsDeleted).Max(m => (int?)m.YearTo) ?? 0;

                var partsList = await _context.ItemMasterlistReferences.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber).ToListAsync();
                var vendorList = await _context.VendorCatalogs.Where(e => e.IsActive && !e.IsDeleted && partsList.Select(e => e.PartsLinkNumber).ToList().Contains(e.PartsLinkNumber)).ToListAsync();

                product.PartsLinkList = partsList;
                product.VendorCatalogList = vendorList;
                product.YearFrom = minYear;
                product.YearTo = maxYear;

                product.OriginalPartNumber = product.PartNumber;
            }

            var result = new PaginatedListDTO<Product>()
            {
                Data = products.OrderBy(e => e.PartNumber).ToList(),
                RecordCount = recordCount
            };

            return result;
        }

        public async Task<List<ProductDTO>> GetProductsList()
        {
            var result = new List<ProductDTO>();

            var products = await _context.Products.Where(p => !p.IsDeleted).ToListAsync();

            foreach (var product in products)
            {
                var partsList = await _context.ItemMasterlistReferences.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber).ToListAsync();
                
                var catalogList = await _context.PartsCatalogs.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber && e.ProductId == product.Id).ToListAsync();
                var partsLinkNumber = string.Empty;
                var OEMNumber = string.Empty;

                if (partsList != null && partsList.Any())
                {
                    var partsLink = partsList.Where(e => e.IsMainPartsLink).FirstOrDefault();
                    partsLinkNumber = partsLink != null ? partsLink.PartsLinkNumber : string.Empty;

                    partsLink = partsList.Where(e => e.IsMainOEM).FirstOrDefault();
                    OEMNumber = partsLink != null ? partsLink.OEMNumber : string.Empty;
                }

                var vendorList = new List<VendorCatalog>();
                if (!string.IsNullOrEmpty(partsLinkNumber))
                {
                    vendorList = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim() == partsLinkNumber.Trim()).ToListAsync();
                }
                
                var productDTO = new ProductDTO()
                {
                    Brand = product.Brand,
                    CategoryId = product.CategoryId,
                    Id = product.Id,
                    ListPrice = product.PriceLevel1,
                    PriceLevel1 = product.PriceLevel1,
                    PriceLevel2 = product.PriceLevel2,
                    PriceLevel3 = product.PriceLevel3,
                    PriceLevel4 = product.PriceLevel4,
                    PriceLevel5 = product.PriceLevel5,
                    PriceLevel6 = product.PriceLevel6,
                    PriceLevel7 = product.PriceLevel7,
                    PriceLevel8 = product.PriceLevel8,  
                    PartDescription = product.PartDescription,
                    PartNumber = product.PartNumber,
                    PartSizeId = product.PartSizeId,
                    SequenceId = product.SequenceId,
                    StatusId = product.StatusId,
                    PartsLinks = (partsList != null && partsList.Any()) ? string.Join(',', partsList.Select(e => e.PartsLinkNumber)) : "",
                    OEMs = (partsList != null && partsList.Any()) ? string.Join(',', partsList.Select(e => e.OEMNumber)) : "",
                    VendorCodes = (vendorList != null && vendorList.Any()) ? string.Join(',', vendorList.Select(e => e.VendorCode)) : "",
                    PartsLinkNumber = partsLinkNumber,
                    OEMNumber = OEMNumber,
                    YearFrom = (catalogList != null && catalogList.Any()) ? catalogList.Min(e => e.YearFrom) : 1900,
                    YearTo = (catalogList != null && catalogList.Any()) ? catalogList.Max(e => e.YearTo) : 1900,
                    VendorCatalogs = vendorList
                };

                result.Add(productDTO);
            }

            return result;
        }
        
        public async Task<PaginatedListDTO<ProductListDTO>> GetSearchProductsListByYearMakeModelPaginated(ProductFilterDTO productFilterDTO, int pageSize, int pageIndex, string? sortColumn = "PartNumber", string? sortOrder = "ASC", string? search = "")
        {
            var productListDTOs = new List<ProductListDTO>();
            var state = productFilterDTO.State;
            var year = productFilterDTO.Year;
            var categoryIds = productFilterDTO.CategoryIds;
            var sequenceIds = productFilterDTO.SequenceIds;
            var make = !string.IsNullOrWhiteSpace(productFilterDTO.Make) ? productFilterDTO.Make.Trim().ToLower() : null;
            var model = !string.IsNullOrWhiteSpace(productFilterDTO.Model) ? productFilterDTO.Model.Trim().ToLower() : null;

            var recordCount = (
                from p in _context.Products
                join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                from c in _context.Categories
                    .Where(e => e.IsActive && !e.IsDeleted && e.Id == p.CategoryId)
                    .DefaultIfEmpty()
                from s in _context.Sequences
                    .Where(e => e.IsActive && !e.IsDeleted && e.Id == p.SequenceId)
                    .DefaultIfEmpty()
                where p.IsActive && !p.IsDeleted &&
                    ((year > 0 ? (pc.YearFrom <= year && pc.YearTo >= year) : true) &&
                      (!string.IsNullOrEmpty(make) ? pc.Make.Trim().ToLower() == make : true) &&
                      (!string.IsNullOrEmpty(model) ? pc.Model.Trim().ToLower() == model : true) &&
                     ((categoryIds != null && categoryIds.Count > 0) ? categoryIds.Contains(p.CategoryId.Value) : true) &&
                     ((sequenceIds != null && sequenceIds.Count > 0) ? sequenceIds.Contains(p.SequenceId.Value) : true))
                select p)
                .Distinct()
                .Count();

            productListDTOs = await (
                from p in _context.Products
                join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                from c in _context.Categories
                    .Where(e => e.IsActive && !e.IsDeleted && e.Id == p.CategoryId)
                    .DefaultIfEmpty()
                from s in _context.Sequences
                    .Where(e => e.IsActive && !e.IsDeleted && e.Id == p.SequenceId)
                    .DefaultIfEmpty()
                where p.IsActive && !p.IsDeleted &&
                    ((year > 0 ? (pc.YearFrom <= year && pc.YearTo >= year) : true) &&
                      (!string.IsNullOrEmpty(make) ? pc.Make.Trim().ToLower() == make : true) &&
                      (!string.IsNullOrEmpty(model) ? pc.Model.Trim().ToLower() == model : true) &&
                     ((categoryIds != null && categoryIds.Count > 0) ? categoryIds.Contains(p.CategoryId.Value) : true) &&
                     ((sequenceIds != null && sequenceIds.Count > 0) ? sequenceIds.Contains(p.SequenceId.Value) : true))
                select new ProductListDTO
                {
                    Id = p.Id,
                    ImageUrl = p.ImageUrl,
                    PartDescription = p.PartDescription.Trim(),
                    PartNumber = p.PartNumber.Trim(),
                    IsActive = p.IsActive,
                    IsCAProduct = p.IsCAProduct,
                    IsNVProduct = p.IsNVProduct,
                })
                .Distinct()
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            foreach (var product in productListDTOs)
            {
                var catalogList = await _context.PartsCatalogs.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber && e.ProductId == product.Id).ToListAsync();
                product.YearFrom = (catalogList != null && catalogList.Any()) ? catalogList.Min(e => e.YearFrom) : 1900;
                product.YearTo = (catalogList != null && catalogList.Any()) ? catalogList.Max(e => e.YearTo) : 1900;
            }

            var result = new PaginatedListDTO<ProductListDTO>()
            {
                Data = productListDTOs.OrderBy(e => e.PartNumber).ToList(),
                RecordCount = recordCount
            };

            return result;
        }

        public async Task<PaginatedListDTO<ProductListDTO>> GetSearchProductsListByPartNumberPaginated(int state, int pageSize, int pageIndex, string? sortColumn = "PartNumber", string? sortOrder = "ASC", string? search = "")
        {
            var productListDTOs = new List<ProductListDTO>();
            var partNo = !string.IsNullOrWhiteSpace(search) ? search.Trim().ToLower() : null;

            var recordCount = (
                from p in _context.Products
                join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                join imr in _context.ItemMasterlistReferences on p.Id equals imr.ProductId
                from vc in _context.VendorCatalogs
                    .Where(e => e.IsActive && !e.IsDeleted && imr.PartsLinkNumber.Trim().ToLower() == e.PartsLinkNumber.Trim().ToLower())
                    .DefaultIfEmpty()
                    //where p.IsActive && !p.IsDeleted &&
                where !p.IsDeleted &&
                    ((!string.IsNullOrEmpty(partNo) ? p.PartNumber.Trim().ToLower().Contains(partNo) : true) ||
                     (!string.IsNullOrEmpty(partNo) ? imr.PartsLinkNumber.Trim().ToLower().Contains(partNo) : true) ||
                     (!string.IsNullOrEmpty(partNo) ? imr.OEMNumber.Trim().ToLower().Contains(partNo) : true) ||
                     (!string.IsNullOrEmpty(partNo) ? !string.IsNullOrEmpty(vc.VendorPartNumber) ? vc.VendorPartNumber.Trim().ToLower().Contains(partNo) : false : true))
                select p)
                .Distinct()
                .Count();

            productListDTOs = await (
                from p in _context.Products
                join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                join imr in _context.ItemMasterlistReferences on p.Id equals imr.ProductId
                from vc in _context.VendorCatalogs
                    .Where(e => e.IsActive && !e.IsDeleted && imr.PartsLinkNumber.Trim().ToLower() == e.PartsLinkNumber.Trim().ToLower())
                    .DefaultIfEmpty()
                    //where p.IsActive && !p.IsDeleted &&
                where !p.IsDeleted &&
                    ((!string.IsNullOrEmpty(partNo) ? p.PartNumber.Trim().ToLower().Contains(partNo) : true) ||
                     (!string.IsNullOrEmpty(partNo) ? imr.PartsLinkNumber.Trim().ToLower().Contains(partNo) : true) ||
                     (!string.IsNullOrEmpty(partNo) ? imr.OEMNumber.Trim().ToLower().Contains(partNo) : true) ||
                     (!string.IsNullOrEmpty(partNo) ? !string.IsNullOrEmpty(vc.VendorPartNumber) ? vc.VendorPartNumber.Trim().ToLower().Contains(partNo) : false : true))
                select new ProductListDTO
                {
                    Id = p.Id,
                    ImageUrl = p.ImageUrl,
                    PartDescription = p.PartDescription.Trim(),
                    PartNumber = p.PartNumber.Trim(),
                    IsActive = p.IsActive,
                    IsCAProduct = p.IsCAProduct,
                    IsNVProduct = p.IsNVProduct,
                })
                .Distinct()
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            foreach (var product in productListDTOs)
            {
                //var prod = await _context.Products.FirstOrDefaultAsync(e => e.Id == product.Id);
                //if (prod != null)
                //{
                //    product.PartDescription = prod.PartDescription;
                //    product.PartNumber = prod.PartNumber;
                //}

                var catalogList = await _context.PartsCatalogs.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber && e.ProductId == product.Id).ToListAsync();
                if (catalogList.Any()) 
                {
                    product.YearFrom = catalogList.Min(e => e.YearFrom);
                    product.YearTo = catalogList.Max(e => e.YearTo);
                }
                else
                {
                    product.YearFrom = 1900;
                    product.YearTo = 1900;
                }
            }

            var result = new PaginatedListDTO<ProductListDTO>()
            {
                Data = productListDTOs.OrderBy(e => e.PartNumber).ToList(),
                RecordCount = recordCount
            };

            return result;
        }

        public async Task<PaginatedListDTO<ProductDTO>> GetProductsListFilteredPaginated(ProductFilterDTO productFilterDTO, int pageSize, int pageIndex, string? sortColumn = "PartNumber", string? sortOrder = "ASC", string? search = "")
        {
            var productDTOs = new List<ProductDTO>();
            var year = productFilterDTO.Year;
            var categoryIds = productFilterDTO.CategoryIds;
            var sequenceIds = productFilterDTO.SequenceIds;
            var make = !string.IsNullOrWhiteSpace(productFilterDTO.Make) ? productFilterDTO.Make.Trim().ToLower() : null;
            var model = !string.IsNullOrWhiteSpace(productFilterDTO.Model) ? productFilterDTO.Model.Trim().ToLower() : null;

            var recordCount = (
                from p in _context.Products
                join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                from c in _context.Categories
                    .Where(e => e.IsActive && !e.IsDeleted && e.Id == p.CategoryId)
                    .DefaultIfEmpty()
                from s in _context.Sequences
                    .Where(e => e.IsActive && !e.IsDeleted && e.Id == p.SequenceId)
                    .DefaultIfEmpty()
                where p.IsActive && !p.IsDeleted &&
                     (((year == null || year == 0) ? true : (pc.YearFrom <= year && pc.YearTo >= year)) &&
                      (string.IsNullOrEmpty(make) ? true : pc.Make.Trim().ToLower() == make) &&
                      (string.IsNullOrEmpty(model) ? true : pc.Model.Trim().ToLower() == model) &&
                      ((categoryIds == null || categoryIds.Count == 0) ? true : categoryIds.Contains(p.CategoryId.Value)) &&
                      ((sequenceIds == null || sequenceIds.Count == 0) ? true : sequenceIds.Contains(p.SequenceId.Value)))
                select p)
                .Distinct()
                .Count();

            productDTOs = await (
                from p in _context.Products
                join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                from c in _context.Categories
                    .Where(e => e.IsActive && !e.IsDeleted && e.Id == p.CategoryId)
                    .DefaultIfEmpty()
                from s in _context.Sequences
                    .Where(e => e.IsActive && !e.IsDeleted && e.Id == p.SequenceId)
                    .DefaultIfEmpty()
                where p.IsActive && !p.IsDeleted &&
                    (((year == null || year == 0) ? true : (pc.YearFrom <= year && pc.YearTo >= year)) &&
                    (string.IsNullOrEmpty(make) ? true : pc.Make.Trim().ToLower() == make) &&
                    (string.IsNullOrEmpty(model) ? true : pc.Model.Trim().ToLower() == model) &&
                    ((categoryIds == null || categoryIds.Count == 0) ? true : categoryIds.Contains(p.CategoryId.Value)) &&
                    ((sequenceIds == null || sequenceIds.Count == 0) ? true : sequenceIds.Contains(p.SequenceId.Value)))
                select new ProductDTO
                {
                    Brand = p.Brand,
                    CategoryId = p.CategoryId,
                    Id = p.Id,
                    ListPrice = p.PriceLevel1,
                    PartDescription = p.PartDescription.Trim(),
                    PartNumber = p.PartNumber.Trim(),
                    PartSizeId = p.PartSizeId,
                    PriceLevel1 = p.PriceLevel1,
                    PriceLevel2 = p.PriceLevel2,
                    PriceLevel3 = p.PriceLevel3,
                    PriceLevel4 = p.PriceLevel4,
                    PriceLevel5 = p.PriceLevel5,
                    PriceLevel6 = p.PriceLevel6,
                    PriceLevel7 = p.PriceLevel7,
                    PriceLevel8 = p.PriceLevel8,
                    SequenceId = p.SequenceId,
                    StatusId = p.StatusId,
                    CurrentCost = p.CurrentCost.HasValue ? p.CurrentCost.Value : 0
                })
                .Distinct()
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            foreach (var product in productDTOs)
            {
                var partsList = await _context.ItemMasterlistReferences.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber).ToListAsync();

                var catalogList = await _context.PartsCatalogs.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber && e.ProductId == product.Id).ToListAsync();
                var partsLinkNumber = string.Empty;
                var OEMNumber = string.Empty;

                if (partsList != null && partsList.Any())
                {
                    var partsLink = partsList.Where(e => e.IsMainPartsLink).FirstOrDefault();
                    partsLinkNumber = partsLink != null ? partsLink.PartsLinkNumber : string.Empty;

                    partsLink = partsList.Where(e => e.IsMainOEM).FirstOrDefault();
                    OEMNumber = partsLink != null ? partsLink.OEMNumber : string.Empty;
                }

                var vendorList = new List<VendorCatalog>();

                if (!string.IsNullOrEmpty(partsLinkNumber))
                {
                    vendorList = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim().ToLower() == partsLinkNumber.Trim().ToLower()).ToListAsync();
                }

                //Get Warehouse Stocks with Location Name
                var stocks = (
                    from stock in _context.WarehouseStocks
                    join location in _context.WarehouseLocations on stock.WarehouseLocationId equals location.Id
                    where stock.ProductId == product.Id
                    select new WarehouseStock
                    {
                        Id = stock.Id,
                        CreatedBy = stock.CreatedBy,
                        CreatedDate = stock.CreatedDate,
                        IsActive = stock.IsActive,
                        IsDeleted = stock.IsDeleted,
                        Location = location.Location,
                        ModifiedBy = stock.ModifiedBy,
                        ModifiedDate = stock.ModifiedDate,
                        ProductId = product.Id,
                        Quantity = stock.Quantity > 0 ? stock.Quantity : 0,
                        WarehouseId = stock.WarehouseId,
                        WarehouseLocationId = stock.WarehouseLocationId,
                        CurrentCost = product.CurrentCost
                    }).ToList();

                // Get Stocks Quantity
                //var stocks = await _context.WarehouseStocks.Where(e => e.ProductId == product.Id).ToListAsync();
                int stockCount = (stocks != null && stocks.Count > 0) ? stocks.Sum(s => s.Quantity) : 0;

                product.PartsLinkNumber = partsLinkNumber;
                product.OEMNumber = OEMNumber;
                product.PartsLinks = (partsList != null && partsList.Any()) ? string.Join(',', partsList.Select(e => e.PartsLinkNumber)) : "";
                product.OEMs = (partsList != null && partsList.Any()) ? string.Join(',', partsList.Select(e => e.OEMNumber)) : "";
                product.VendorCodes = (vendorList != null && vendorList.Any()) ? string.Join(',', vendorList.Select(e => e.VendorCode)) : "";
                product.YearFrom = (catalogList != null && catalogList.Any()) ? catalogList.Min(e => e.YearFrom) : 1900;
                product.YearTo = (catalogList != null && catalogList.Any()) ? catalogList.Max(e => e.YearTo) : 1900;
                product.VendorCatalogs = vendorList;
                product.Stock = stockCount;
                product.Stocks = stocks;
            }

            var result = new PaginatedListDTO<ProductDTO>()
            {
                Data = productDTOs.OrderBy(e => e.PartNumber).ToList(),
                RecordCount = recordCount
            };

            return result;
        }

        public async Task<PaginatedListDTO<ProductDTO>> GetProductsListByPartNumberPaginated(string? partNumber, int pageSize, int pageIndex, string? sortColumn = "PartNumber", string? sortOrder = "ASC", string? search = "")
        {
            var productDTOs = new List<ProductDTO>();
            var partNo = !string.IsNullOrWhiteSpace(partNumber) ? partNumber.Trim().ToLower() : null;

            var recordCount = (
                from p in _context.Products
                join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                join imr in _context.ItemMasterlistReferences on p.Id equals imr.ProductId
                //join vc in _context.VendorCatalogs on imr.PartsLinkNumber.Trim().ToLower() equals vc.PartsLinkNumber.Trim().ToLower()
                where p.IsActive && !p.IsDeleted && 
                    ((string.IsNullOrEmpty(partNo) ? true : p.PartNumber.Trim().ToLower().Contains(partNo)) ||
                     (string.IsNullOrEmpty(partNo) ? true : imr.PartsLinkNumber.Trim().ToLower().Contains(partNo)) ||
                     (string.IsNullOrEmpty(partNo) ? true : imr.OEMNumber.Trim().ToLower().Contains(partNo))) // ||
                     //(string.IsNullOrEmpty(partNo) ? true : vc.VendorPartNumber.Trim().ToLower().Contains(partNo)))
                select p)
                .Distinct()
                .Count();

            productDTOs = await(
                from p in _context.Products
                join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                join imr in _context.ItemMasterlistReferences on p.Id equals imr.ProductId
                //join vc in _context.VendorCatalogs on imr.PartsLinkNumber.Trim().ToLower() equals vc.PartsLinkNumber.Trim().ToLower()
                where p.IsActive && !p.IsDeleted &&
                    ((string.IsNullOrEmpty(partNo) ? true : p.PartNumber.Trim().ToLower().Contains(partNo)) ||
                     (string.IsNullOrEmpty(partNo) ? true : imr.PartsLinkNumber.Trim().ToLower().Contains(partNo)) ||
                     (string.IsNullOrEmpty(partNo) ? true : imr.OEMNumber.Trim().ToLower().Contains(partNo))) // ||
                     //(string.IsNullOrEmpty(partNo) ? true : vc.VendorPartNumber.Trim().ToLower().Contains(partNo)))
                select new ProductDTO
                {
                    Brand = p.Brand,
                    CategoryId = p.CategoryId,
                    Id = p.Id,
                    ListPrice = p.PriceLevel1,
                    PartDescription = p.PartDescription.Trim(),
                    PartNumber = p.PartNumber.Trim(),
                    PartSizeId = p.PartSizeId,
                    PriceLevel1 = p.PriceLevel1,
                    PriceLevel2 = p.PriceLevel2,
                    PriceLevel3 = p.PriceLevel3,
                    PriceLevel4 = p.PriceLevel4,
                    PriceLevel5 = p.PriceLevel5,
                    PriceLevel6 = p.PriceLevel6,
                    PriceLevel7 = p.PriceLevel7,
                    PriceLevel8 = p.PriceLevel8,
                    SequenceId = p.SequenceId,
                    StatusId = p.StatusId,
                    CurrentCost = p.CurrentCost.HasValue ? p.CurrentCost.Value : 0
                })
                .Distinct()
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            foreach (var product in productDTOs)
            {
                var partsList = await _context.ItemMasterlistReferences.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber).ToListAsync();

                var catalogList = await _context.PartsCatalogs.Where(e => e.IsActive && !e.IsDeleted && e.PartNumber == product.PartNumber && e.ProductId == product.Id).ToListAsync();
                var partsLinkNumber = string.Empty;
                var OEMNumber = string.Empty;

                if (partsList != null && partsList.Any())
                {
                    var partsLink = partsList.Where(e => e.IsMainPartsLink).FirstOrDefault();
                    partsLinkNumber = partsLink != null ? partsLink.PartsLinkNumber : string.Empty;

                    partsLink = partsList.Where(e => e.IsMainOEM).FirstOrDefault();
                    OEMNumber = partsLink != null ? partsLink.OEMNumber : string.Empty;
                }

                var vendorList = new List<VendorCatalog>();
                
                if (!string.IsNullOrEmpty(partsLinkNumber))
                {
                    vendorList = await _context.VendorCatalogs.Where(e => e.PartsLinkNumber.Trim().ToLower() == partsLinkNumber.Trim().ToLower()).ToListAsync();
                }

                //Get Warehouse Stocks with Location Name
                var stocks = (
                    from stock in _context.WarehouseStocks
                    join location in _context.WarehouseLocations on stock.WarehouseLocationId equals location.Id
                    where stock.ProductId == product.Id
                    select new WarehouseStock
                    {
                        Id = stock.Id,
                        CreatedBy = stock.CreatedBy,
                        CreatedDate = stock.CreatedDate,
                        IsActive = stock.IsActive,
                        IsDeleted = stock.IsDeleted,
                        Location = location.Location,
                        ModifiedBy = stock.ModifiedBy,
                        ModifiedDate = stock.ModifiedDate,
                        ProductId = product.Id,
                        Quantity = stock.Quantity > 0 ? stock.Quantity : 0,
                        WarehouseId = stock.WarehouseId,
                        WarehouseLocationId = stock.WarehouseLocationId,
                        CurrentCost = product.CurrentCost
                    }).ToList();

                // Get Stocks Quantity
                // var stocks = await _context.WarehouseStocks.Where(e => e.ProductId == product.Id).ToListAsync();
                int stockCount = (stocks != null && stocks.Count > 0) ? stocks.Sum(s => s.Quantity) : 0;

                product.PartsLinkNumber = partsLinkNumber;
                product.OEMNumber = OEMNumber;
                product.PartsLinks = (partsList != null && partsList.Any()) ? string.Join(',', partsList.Select(e => e.PartsLinkNumber)) : "";
                product.OEMs = (partsList != null && partsList.Any()) ? string.Join(',', partsList.Select(e => e.OEMNumber)) : "";
                product.VendorCodes = (vendorList != null && vendorList.Any()) ? string.Join(',', vendorList.Select(e => e.VendorCode)) : "";
                product.YearFrom = (catalogList != null && catalogList.Any()) ? catalogList.Min(e => e.YearFrom) : 1900;
                product.YearTo = (catalogList != null && catalogList.Any()) ? catalogList.Max(e => e.YearTo) : 1900;
                product.VendorCatalogs = vendorList;
                product.Stock = stockCount;
                product.Stocks = stocks;
            }

            var result = new PaginatedListDTO<ProductDTO>()
            {
                Data = productDTOs.OrderBy(e => e.PartNumber).ToList(),
                RecordCount = recordCount
            };

            return result;
        }


        public async Task<Product> GetSingleProduct(string searchKey)
        {
            var result = new Product();

            result = await (
                from p in _context.Products
                join iml in _context.ItemMasterlistReferences on p.Id equals iml.ProductId
                join pc in _context.PartsCatalogs on p.Id equals pc.ProductId
                where p.IsActive && !p.IsDeleted && (
                    p.PartNumber.ToLower().Trim().Contains(searchKey.ToLower().Trim()) ||
                    p.PartDescription.ToLower().Trim().Contains(searchKey.ToLower().Trim()) ||
                    p.PartsLinkNumber.ToLower().Trim().Contains(searchKey.ToLower().Trim()) ||
                    p.OEMNumber.ToLower().Trim().Contains(searchKey.ToLower().Trim()) ||
                    iml.PartsLinkNumber.ToLower().Trim().Contains(searchKey.ToLower().Trim()) ||
                    iml.OEMNumber.ToLower().Trim().Contains(searchKey.ToLower().Trim()))
                select new Product
                {
                    Id = p.Id,
                    Brand = p.Brand,
                    CategoryId = p.CategoryId,
                    CopyType = p.CopyType,
                    CreatedBy = p.CreatedBy,
                    CreatedDate = p.CreatedDate, 
                    CurrentCost = p.CurrentCost,
                    DateAdded = p.DateAdded,
                    Image = p.Image,
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive,
                    IsAPIAllowed = p.IsAPIAllowed,
                    IsCAProduct = p.IsCAProduct,
                    IsDeleted = p.IsDeleted,
                    IsDropShipAllowed = p.IsDropShipAllowed,
                    IsNVProduct = p.IsNVProduct,
                    IsWebsiteOption = p.IsWebsiteOption,
                    ModifiedBy = p.ModifiedBy,
                    ModifiedDate = p.ModifiedDate,
                    OEMListPrice = p.OEMListPrice,
                    OEMNumber = p.OEMNumber,
                    //OEMs = p.OEMs,
                    OnOrder = p.OnOrder,
                    OnReceivingHold = p.OnReceivingHold,
                    OriginalPartNumber = p.OriginalPartNumber,
                    PartDescription = p.PartDescription,
                    PartNumber = p.PartNumber,
                    PartSizeId = p.PartSizeId,
                    PartsLinkList = p.PartsLinkList,
                    PartsLinkNumber = p.PartsLinkNumber,
                    //PartsLinks  = p.PartsLinks,
                    PreviousCost = p.PreviousCost,
                    PriceLevel1 = p.PriceLevel1,
                    PriceLevel2 = p.PriceLevel2,
                    PriceLevel3 = p.PriceLevel3,
                    PriceLevel4 = p.PriceLevel4,    
                    PriceLevel5 = p.PriceLevel5,
                    PriceLevel6 = p.PriceLevel6,
                    PriceLevel7 = p.PriceLevel7,
                    PriceLevel8 = p.PriceLevel8,
                    SequenceId = p.SequenceId,
                    StatusId    = p.StatusId,
                    StockQuantity = p.StockQuantity,
                    VendorCatalogList   = p.VendorCatalogList,
                    //VendorCodes = p.VendorCodes,
                    //VendorPartNumbers = p.VendorPartNumbers,
                    YearFrom = p.YearFrom,
                    YearTo = p.YearTo
                })
                .Distinct()
                .FirstOrDefaultAsync();

            return result != null ? result : new Product(); ;
        }

        #endregion

        #region Save Data
        public async Task<List<Product>> Create(Product product)
        {
            // Check if Product is a Copy
            if (!string.IsNullOrEmpty(product.CopyType))
            {
                var exist = await _context.Products.FirstOrDefaultAsync(e => e.PartNumber == product.PartNumber);
                if (exist != null)
                {
                    return new List<Product>();
                }
            }

            // Get Parts Catalogs
            var partsCatalogs = await _context.PartsCatalogs.Where(e => e.ProductId == product.Id).ToListAsync();

            product.Id = 0;
            _context.Products.Add(product);
            await _context.SaveEntitiesAsync();

            if (product.PartsLinkList == null)
            {
                var partsLink = new ItemMasterlistReference()
                {
                    CreatedBy = product.CreatedBy,
                    CreatedDate = product.CreatedDate,
                    IsActive = product.IsActive,
                    IsDeleted = product.IsDeleted,
                    IsMainOEM = true,
                    IsMainPartsLink = true,
                    OEMNumber = product.OEMNumber,
                    PartNumber = product.PartNumber,
                    PartsLinkNumber = product.PartsLinkNumber,
                    ProductId = product.Id
                };

                await _context.ItemMasterlistReferences.AddAsync(partsLink);
            }
            else
            {
                foreach (var part in product.PartsLinkList)
                {
                    var partsLink = new ItemMasterlistReference()
                    {
                        CreatedBy = product.CreatedBy,
                        CreatedDate = product.CreatedDate,
                        IsActive = part.IsActive,
                        IsDeleted = part.IsDeleted,
                        IsMainOEM = part.IsMainOEM,
                        OEMNumber = part.OEMNumber,
                        IsMainPartsLink = part.IsMainPartsLink,
                        PartsLinkNumber = part.PartsLinkNumber,
                        PartNumber = product.PartNumber,
                        ProductId = product.Id
                    };

                    await _context.ItemMasterlistReferences.AddAsync(partsLink);
                }
            }

            foreach (var part in partsCatalogs)
            {
                var pc = new PartsCatalog()
                {
                    Id = 0,
                    Brand = part.Brand,
                    CreatedBy = product.CreatedBy,
                    CreatedDate = product.CreatedDate,
                    Cylinder = part.Cylinder,
                    GroupHead = part.GroupHead,
                    IsActive = part.IsActive,
                    IsDeleted = part.IsDeleted,
                    Liter = part.Liter,
                    Make = part.Make,
                    Model = part.Model,
                    Notes = part.Notes,
                    PartNumber = product.PartNumber,
                    Position = part.Position,
                    ProductId = product.Id,
                    SubGroup = part.SubGroup,
                    SubModel = part.SubModel,
                    YearFrom = part.YearFrom,
                    YearTo = part.YearTo,
                };

                await _context.PartsCatalogs.AddAsync(pc);
            }

            await _context.SaveEntitiesAsync();

            return new List<Product>(); // await _context.Products.ToListAsync(); // GetProducts(); // 
        }

        public async Task<List<Product>> Update(Product product)
        {
            var newPartNo = product.PartNumber;
            var origPartNo = product.OriginalPartNumber;
            _context.Products.Update(product);
            
            if (newPartNo != origPartNo)
            {
                //var orderDetails = await _context.OrderDetails.Where(e => e.ProductId == product.Id).ToListAsync();

                //foreach (var item in orderDetails)
                //{
                //    item.PartNumber = newPartNo;
                //    await _context.OrderDetails.AddAsync(item);
                //}

                var itemMasterLists = await _context.ItemMasterlistReferences.Where(e => e.ProductId == product.Id).ToListAsync();
                foreach (var item in itemMasterLists)
                {
                    item.PartNumber = newPartNo;
                }

                var partsCatalogs = await _context.PartsCatalogs.Where(e => e.ProductId == product.Id).ToListAsync();
                foreach (var item in partsCatalogs)
                {
                    item.PartNumber = newPartNo;
                }

                _context.ItemMasterlistReferences.UpdateRange(itemMasterLists);
                _context.PartsCatalogs.UpdateRange(partsCatalogs);
            }
            
            await _context.SaveEntitiesAsync();
            return new List<Product>(); //  await _context.Products.ToListAsync();  //GetProducts(); // _context.Products.ToListAsync();
        }

        public async Task<List<Product>> Delete(List<int> productIds)
        {
            var products = _context.Products.Where(a => productIds.Contains(a.Id)).ToList();
            _context.Products.RemoveRange(products);
            await _context.SaveEntitiesAsync();
            return new List<Product>(); // await _context.Products.ToListAsync();  //GetProducts(); // _context.Products.ToListAsync();
        }

        public async Task<List<Product>> SoftDelete(List<int> productIds)
        {
            var products = _context.Products.Where(a => productIds.Contains(a.Id)).ToList();
            products.ForEach(a => { a.IsDeleted = true; });

            _context.Products.UpdateRange(products);
            await _context.SaveEntitiesAsync();
            return new List<Product>(); // await _context.Products.ToListAsync(); //GetProducts(); // _context.Products.ToListAsync();
        }

        #endregion
    }
}
