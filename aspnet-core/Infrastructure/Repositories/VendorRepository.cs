using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Repositories
{
    public class VendorRepository : IVendorRepository
    {
        private readonly DataContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public VendorRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Get Data
        public async Task<List<Vendor>> GetVendors()
        {
            return await _context.Vendors.ToListAsync();
        }

        public async Task<List<Vendor>> GetVendorsByState(string state)
        {
            var result = new List<Vendor>();
            if (!string.IsNullOrWhiteSpace(state))
            {
                result = state.Trim().ToLower() == "nv" ? await _context.Vendors.Where(e => e.IsNVVendor).ToListAsync() : await _context.Vendors.Where(e => e.IsCAVendor).ToListAsync();
            }
            return result;
        }

        public async Task<Vendor?> GetVendor(int vendorId)
        {
            var result = await _context.Vendors.FindAsync(vendorId);
            if (result == null)
                return null;

            return result;
        }

        public async Task<List<VendorOrderDTO>> GetVendorOrdersByVendorCode(string vendorCode)
        {
            var result = new List<VendorOrderDTO>();
            try 
            {
                var orderDetails = await (
                    from order in _context.Orders
                    join orderDetail in _context.OrderDetails on order.Id equals orderDetail.OrderId
                    where order.IsActive == true && !order.IsDeleted && !order.IsQuote &&
                        (order.OrderStatusId != 3 && order.OrderStatusId != 5 && order.OrderStatusId != 6) && (orderDetail.StatusId == null || orderDetail.StatusId == 7) &&
                        (orderDetail.StatusId == 7 ? true : (orderDetail.WarehouseTracking == null || orderDetail.WarehouseTracking.Trim() == string.Empty)) &&
                         orderDetail.VendorCode.Trim().ToLower() == vendorCode.Trim().ToLower() && orderDetail.IsActive && !orderDetail.IsDeleted
                    select orderDetail)
                    .ToListAsync();
                
                foreach (var orderDetail in orderDetails)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(e => e.Id == orderDetail.ProductId);
                    var sequence = await _context.Sequences.FirstOrDefaultAsync(e => e.Id == product.SequenceId);
                    var order = await _context.Orders.FirstOrDefaultAsync(e => e.Id == orderDetail.OrderId);

                    if (order != null)
                    {
                        result.Add(new VendorOrderDTO()
                        {
                            OrderId = orderDetail.OrderId,
                            OrderDetailId = orderDetail.Id,
                            MainPartsLinkNumber = orderDetail.MainPartsLinkNumber,
                            Sequence = sequence != null ? sequence.CategoryGroupDescription : "",
                            CreatedBy = orderDetail.CreatedBy,
                            CreatedDate = orderDetail.CreatedDate.ToLocalTime(),
                            DeliveryMethod = order.DeliveryType == 1 ? "Delivery" : order.DeliveryType == 2 ? "Pickup" : "Shipping",
                            OrderDate = order.OrderDate.ToLocalTime(),
                            Id = orderDetail.Id,
                            IsActive = orderDetail.IsActive,
                            IsDeleted = orderDetail.IsDeleted,
                            ModifiedBy = orderDetail.ModifiedBy,
                            ModifiedDate = orderDetail.ModifiedDate,
                            OrderNumber = order != null ? (order.OrderNumber != null ? order.OrderNumber.Value : 0) : 0,
                            PurchaseOrderNumber = order != null ? (order.PurchaseOrderNumber != null ? order.PurchaseOrderNumber : "") : "",
                            OrderQuantity = orderDetail.OrderQuantity,
                            PartDescription = orderDetail.PartDescription,
                            PartNumber = orderDetail.PartNumber,
                            PODetailStatus = "",
                            ShipZone = order != null ? order.ShipZone : "",
                            VendorPartNumber = orderDetail.VendorPartNumber,
                            VendorPrice = orderDetail.VendorPrice,
                            CustomerName = order != null ? order.ShipAddressName : "",
                            DeliveryDate = order != null ? order.DeliveryDate.ToLocalTime() : DateTime.MinValue,
                            DeliveryRoute = order != null ? order.DeliveryRoute != null ? order.DeliveryRoute : 0 : 0
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }
        #endregion

        #region Save Data
        public async Task<List<Vendor>> Create(Vendor vendor)
        {
            _context.Vendors.Add(vendor);
            await _context.SaveEntitiesAsync();

            //Rearrange CA Ranking
            if (vendor.CARank > 0)
            {
                var highVendors = await _context.Vendors.Where(e => e.IsCAVendor && e.Id != vendor.Id && e.CARank >= vendor.CARank).OrderBy(o => o.CARank).ToListAsync();

                if (highVendors.Any())
                {
                    for (int i = 0; i < highVendors.Count; i++)
                    {
                        var currVendor = highVendors[i];
                        currVendor.CARank = vendor.CARank + 1 + i;
                        _context.Vendors.Update(currVendor);
                    }

                    await _context.SaveEntitiesAsync();
                }
            }

            //Rearrange NV Ranking
            if (vendor.NVRank > 0)
            {
                var highVendors = await _context.Vendors.Where(e => e.IsNVVendor && e.Id != vendor.Id && e.NVRank >= vendor.NVRank).OrderBy(o => o.NVRank).ToListAsync();

                if (highVendors.Any())
                {
                    for (int i = 0; i < highVendors.Count; i++)
                    {
                        var currVendor = highVendors[i];
                        currVendor.NVRank = vendor.NVRank + 1 + i;
                        _context.Vendors.Update(currVendor);
                    }

                    await _context.SaveEntitiesAsync();
                }
            }

            return await _context.Vendors.ToListAsync();
        }

        public async Task<List<Vendor>> Update(Vendor vendor)
        {
            var origVendor = await _context.Vendors.AsNoTracking().FirstOrDefaultAsync(e => e.Id == vendor.Id);
            if (origVendor != null)
            {
                _context.Vendors.Update(vendor);

                //Rearrange CA Ranking
                if (vendor.CARank != origVendor.CARank) 
                { 
                    if (vendor.CARank > 0)
                    {
                        var highVendors = await _context.Vendors.Where(e => e.IsCAVendor && e.Id != vendor.Id && e.CARank >= vendor.CARank).OrderBy(o => o.CARank).ToListAsync();
                        
                        if (highVendors.Any())
                        {
                            for (int i = 0; i < highVendors.Count; i++)
                            {
                                var currVendor = highVendors[i];
                                currVendor.CARank = vendor.CARank + 1 + i;
                                _context.Vendors.Update(currVendor);
                            }

                            await _context.SaveEntitiesAsync();
                        }
                    }
                    else
                    {
                        var caVendors = await _context.Vendors.Where(e => e.IsCAVendor && e.Id != vendor.Id).OrderBy(o => o.CARank).ToListAsync();
                        if (caVendors.Any())
                        {
                            for (int i = 0; i < caVendors.Count; i++)
                            {
                                var currVendor = caVendors[i];
                                if (currVendor.CARank != i + 1)
                                {
                                    currVendor.CARank = i + 1;
                                    _context.Vendors.Update(currVendor);
                                }
                            }

                            await _context.SaveEntitiesAsync();
                        }
                    }
                }

                //Rearrange NV Ranking
                if (vendor.NVRank != origVendor.NVRank)
                {
                    if (vendor.NVRank > 0)
                    {
                        var highVendors = await _context.Vendors.Where(e => e.IsNVVendor && e.Id != vendor.Id && e.NVRank >= vendor.NVRank).OrderBy(o => o.NVRank).ToListAsync();

                        if (highVendors.Any())
                        {
                            for (int i = 0; i < highVendors.Count; i++)
                            {
                                var currVendor = highVendors[i];
                                currVendor.NVRank = vendor.NVRank + 1 + i;
                                _context.Vendors.Update(currVendor);
                            }

                            await _context.SaveEntitiesAsync();
                        }
                    }
                    else
                    {
                        var caVendors = await _context.Vendors.Where(e => e.IsNVVendor && e.Id != vendor.Id).OrderBy(o => o.NVRank).ToListAsync();
                        if (caVendors.Any())
                        {
                            for (int i = 0; i < caVendors.Count; i++)
                            {
                                var currVendor = caVendors[i];
                                if (currVendor.NVRank != i + 1)
                                {
                                    currVendor.NVRank = i + 1;
                                    _context.Vendors.Update(currVendor);
                                }
                            }

                            await _context.SaveEntitiesAsync();
                        }
                    }
                }
            }

            await _context.SaveEntitiesAsync();
            return await _context.Vendors.ToListAsync();
        }

        public async Task<List<Vendor>> Delete(List<int> vendorIds)
        {
            var vendors = _context.Vendors.Where(a => vendorIds.Contains(a.Id)).ToList();
            _context.Vendors.RemoveRange(vendors);
            await _context.SaveEntitiesAsync();
            return await _context.Vendors.ToListAsync();
        }

        public async Task<List<Vendor>> SoftDelete(List<int> vendorIds)
        {
            var vendors = _context.Vendors.Where(a => vendorIds.Contains(a.Id)).ToList();
            vendors.ForEach(a => { a.IsDeleted = true; });

            _context.Vendors.UpdateRange(vendors);
            await _context.SaveEntitiesAsync();
            return await _context.Vendors.ToListAsync();
        }
#endregion
    }
}
