using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO.Paginated;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class PartsManifestRepository : IPartsManifestRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public PartsManifestRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<PartsManifest>> GetPartsManifests()
        {
            return await _context.PartsManifests.ToListAsync();
        }

        public async Task<PartsManifest?> GetPartsManifest(int partsManifestId)
        {
            var result = await _context.PartsManifests.FindAsync(partsManifestId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<PaginatedListDTO<PartsManifest>> GetPartsManifestsPaginated(int pageSize, int pageIndex, string? sortColumn, string? sortOrder, string? search)
        {
            var recordCount = (
                from pm in _context.PartsManifests
                join pmd in _context.PartsManifestDetails on pm.Id equals pmd.PartsManifestId
                join p in _context.Products on pmd.ProductId equals p.Id
                join imr in _context.ItemMasterlistReferences on p.Id equals imr.ProductId
                from vc in _context.VendorCatalogs
                    .Where(e => e.IsActive && !e.IsDeleted && imr.PartsLinkNumber.Trim().ToLower() == e.PartsLinkNumber.Trim().ToLower())
                    .DefaultIfEmpty()
                //join itemMasterlistReference in _context.ItemMasterlistReferences on pmd.ProductId equals itemMasterlistReference.ProductId
                //from vendorCatalog in _context.VendorCatalogs
                //    .Where(e => e.IsActive && !e.IsDeleted && (itemMasterlistReference.PartsLinkNumber != null ? e.PartsLinkNumber.Trim().ToLower() == itemMasterlistReference.PartsLinkNumber.Trim().ToLower() : true))
                //    .DefaultIfEmpty()
                where pm.IsActive == true && pm.IsDeleted == false &&
                    (string.IsNullOrEmpty(search) ? true : pm.PurposeName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : pm.VendorCode == null ? false : pm.VendorCode.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : pm.VendorName == null ? false : pm.VendorName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : pm.DriverName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : pm.PartsManifestNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : pmd.PartNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : pmd.PartDescription.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : imr.PartsLinkNumber == null ? false : imr.PartsLinkNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : imr.OEMNumber == null ? false : imr.OEMNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : vc.VendorPartNumber.Trim().ToLower().Contains(search)
                     )
                select pm)
                .Distinct()
                .Count();

            var partsManifests = await (
                from pm in _context.PartsManifests
                join pmd in _context.PartsManifestDetails on pm.Id equals pmd.PartsManifestId
                join p in _context.Products on pmd.ProductId equals p.Id
                join imr in _context.ItemMasterlistReferences on p.Id equals imr.ProductId
                from vc in _context.VendorCatalogs
                    .Where(e => e.IsActive && !e.IsDeleted && imr.PartsLinkNumber.Trim().ToLower() == e.PartsLinkNumber.Trim().ToLower())
                    .DefaultIfEmpty()
                    //join itemMasterlistReference in _context.ItemMasterlistReferences on pmd.ProductId equals itemMasterlistReference.ProductId
                    //from vendorCatalog in _context.VendorCatalogs
                    //    .Where(e => e.IsActive && !e.IsDeleted && (itemMasterlistReference.PartsLinkNumber != null ? e.PartsLinkNumber.Trim().ToLower() == itemMasterlistReference.PartsLinkNumber.Trim().ToLower() : true))
                    //    .DefaultIfEmpty()
                where pm.IsActive == true && pm.IsDeleted == false &&
                    (string.IsNullOrEmpty(search) ? true : pm.PurposeName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : pm.VendorCode == null ? false : pm.VendorCode.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : pm.VendorName == null ? false : pm.VendorName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : pm.DriverName.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : pm.PartsManifestNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : pmd.PartNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : pmd.PartDescription.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : imr.PartsLinkNumber == null ? false : imr.PartsLinkNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : imr.OEMNumber == null ? false : imr.OEMNumber.Trim().ToLower().Contains(search) ||
                     string.IsNullOrEmpty(search) ? true : vc.VendorPartNumber.Trim().ToLower().Contains(search)
                     )
                select pm)
                .Distinct()
                .OrderByDescending(e => e.Id)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            foreach (var partsManifest in partsManifests)
            {
                var partsManifestDetails = await _context.PartsManifestDetails.Where(e => e.PartsManifestId == partsManifest.Id && e.IsActive && !e.IsDeleted).ToListAsync();
                if (partsManifestDetails.Any())
                {
                    partsManifest.PartsManifestDetails = partsManifestDetails;
                }
            }

            var result = new PaginatedListDTO<PartsManifest>()
            {
                Data = partsManifests,
                RecordCount = recordCount
            };

            return result;
        }

        public async Task<PaginatedListDTO<PartsManifest>> GetPartsManifestsByDatePaginated(int pageSize, int pageIndex, DateTime fromDate, DateTime toDate)
        {
            DateTime fDate = fromDate.ToUniversalTime();
            DateTime tDate = toDate.ToUniversalTime();

            var recordCount = (
                from partsManifest in _context.PartsManifests
                where partsManifest.IsActive == true && partsManifest.IsDeleted == false && 
                    (partsManifest.CreatedDate.Date >= fDate.Date && partsManifest.CreatedDate.Date <= tDate.Date)
                select partsManifest)
                .Distinct()
                .Count();

            var partsManifests = await (
                from partsManifest in _context.PartsManifests
                where partsManifest.IsActive == true && partsManifest.IsDeleted == false && 
                    (partsManifest.CreatedDate.Date >= fDate.Date && partsManifest.CreatedDate.Date <= tDate.Date)
                select partsManifest)
                .Distinct()
                .OrderByDescending(e => e.CreatedDate)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            foreach (var partsManifest in partsManifests)
            {
                var partsManifestDetails = await _context.PartsManifestDetails.Where(e => e.PartsManifestId == partsManifest.Id && e.IsActive && !e.IsDeleted).ToListAsync();
                // Get distinct Order Ids
                if (partsManifestDetails != null)
                {
                    //List<int> orderIds = partsManifestDetails.DistinctBy(e => e.OrderId).Select(f => f.OrderId).ToList();

                    //foreach (var orderId in orderIds)
                    //{
                    //    // Get Totals per Order
                    //    var totalAmount = partsManifestDetails.Where(e => e.OrderId == orderId).Sum(e => e.TotalAmount);

                    //    if (partsManifestDetails.FirstOrDefault(e => e.OrderId == orderId) != null)
                    //    {
                    //        partsManifestDetails.FirstOrDefault(e => e.OrderId == orderId).OrderTotalAmount = totalAmount;
                    //    }
                    //}

                    partsManifest.PartsManifestDetails = partsManifestDetails;
                }
            }

            var result = new PaginatedListDTO<PartsManifest>()
            {
                Data = partsManifests,
                RecordCount = recordCount
            };

            return result;
        }
        #endregion

        #region Save Data
        public async Task<bool> Create(PartsManifest partsManifest)
        {
            try
            {
                //var trackingStatus = $"DELIVERED BY {partsManifest.DriverName}";

                // Get PFWBNumber Max + 1
                int maxPartsManifestNumber = _context.PartsManifests.Count() > 0 ? _context.PartsManifests.Max(e => Convert.ToInt32(e.PartsManifestNumber.Substring(2))) + 1 : 0;
                partsManifest.PartsManifestNumber = $"PM{maxPartsManifestNumber.ToString("D4")}";

                await _context.PartsManifests.AddAsync(partsManifest);
                await _context.SaveEntitiesAsync();


                //// Update Order status
                //List<int> orderIds = partsManifest.PartsManifestDetails.DistinctBy(e => e.OrderId).Select(f => f.OrderId).ToList();

                //foreach (var orderId in orderIds)
                //{
                //    var order = await _context.Orders.FirstOrDefaultAsync(e => e.Id == orderId);
                //    if (order != null && order.OrderStatusId != 2 && order.OrderStatusId != 9)
                //    {
                //        order.OrderStatusId = 4;
                //        order.OrderStatusName = "Delivered";
                //        _context.Orders.Update(order);
                //    }
                //}


                foreach (var partsManifestDetail in partsManifest.PartsManifestDetails)
                {
                    partsManifestDetail.PartsManifestId = partsManifest.Id;
                    await _context.PartsManifestDetails.AddAsync(partsManifestDetail);

                    //// Add warehouse Tracking
                    //var warehouseTracking = new WarehouseTracking()
                    //{
                    //    CreatedBy = partsManifest.CreatedBy,
                    //    CreatedDate = partsManifest.CreatedDate.ToUniversalTime(),
                    //    Description = trackingStatus,
                    //    Id = 0,
                    //    IsActive = true,
                    //    IsDeleted = false,
                    //    //OrderDetailId = partsManifestDetail.OrderDetailId,
                    //    //OrderId = partsManifestDetail.OrderId,
                    //    Status = trackingStatus
                    //};

                    //await _context.WarehouseTrackings.AddAsync(warehouseTracking);

                    //// Update OrderDetai WarehouseTracking
                    //var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(e => e.Id == partsManifestDetail.OrderDetailId);
                    //if (orderDetail != null)
                    //{
                    //    orderDetail.WarehouseTracking = trackingStatus;
                    //    _context.OrderDetails.Update(orderDetail);
                    //}
                }

                await _context.SaveEntitiesAsync();
                return true;
            }
            catch 
            {
                return false;
            }
        }

        public async Task<List<PartsManifest>> Update(PartsManifest partsManifest)
        {
            // Convert Dates back to UTC prior to saving
            partsManifest.CreatedDate = partsManifest.CreatedDate.ToUniversalTime();

            _context.PartsManifests.Update(partsManifest);
            await _context.SaveEntitiesAsync();
            return await _context.PartsManifests.ToListAsync();
        }

        public async Task<List<PartsManifest>> Delete(List<int> partsManifestIds)
        {
            var partsManifests = _context.PartsManifests.Where(a => partsManifestIds.Contains(a.Id)).ToList();
            _context.PartsManifests.RemoveRange(partsManifests);
            await _context.SaveEntitiesAsync();
            return await _context.PartsManifests.ToListAsync();
        }

        public async Task<List<PartsManifest>> SoftDelete(List<int> partsManifestIds)
        {
            var partsManifests = _context.PartsManifests.Where(a => partsManifestIds.Contains(a.Id)).ToList();
            partsManifests.ForEach(a => { a.IsDeleted = true; });

            _context.PartsManifests.UpdateRange(partsManifests);
            await _context.SaveEntitiesAsync();
            return await _context.PartsManifests.ToListAsync();
        }
        #endregion
    }
}
