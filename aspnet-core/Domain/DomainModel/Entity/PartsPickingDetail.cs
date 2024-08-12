using Domain.DomainModel.Base;
using Domain.DomainModel.Entity.DTO;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(PartsPickingId), nameof(OrderNumber), nameof(PartNumber), nameof(OrderId), nameof(OrderDate), nameof(StatusId), nameof(PPDetailStatus), Name = "IDX_PARTS_PICKING_DETAIL")]
    public class PartsPickingDetail : BaseModel
    {
        private DateTime _orderDate;
        private DateTime _deliveryDate;

        #region Properties
        [ForeignKey("FK_PartsPickingDetail_PartsPickingId")]
        public int PartsPickingId { get; set; }
        public PartsPicking? PartsPicking { get; set; }

        [ForeignKey("FK_PartsPickingDetail_OrderDetailId")]
        public int OrderDetailId { get; set; }
        public OrderDetail? OrderDetail { get; set; }

        [ForeignKey("FK_PartsPickingDetail_ProductId")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int OrderId { get; set; }
        public int OrderNumber { get; set; }

        public int OrderQuantity { get; set; }
        public int StockQuantity { get; set; }
        
        [MaxLength(20)]
        public string StockLocation { get; set; } = string.Empty;

        [MaxLength(5)]
        public string ShipZone { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PartNumber { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Sequence { get; set; } = string.Empty;

        [MaxLength(200)]
        public string PartDescription { get; set; } = string.Empty;

        public DateTime OrderDate
        {
            get
            {
                return this._orderDate.ToLocalTime();
            }
            set
            {
                this._orderDate = value;
            }
        }

        public DateTime DeliveryDate
        {
            get
            {
                return this._deliveryDate.ToLocalTime();
            }
            set
            {
                this._deliveryDate = value;
            }
        }

        public int DeliveryRoute { get; set; }

        public int StatusId { get; set; }

        [MaxLength(100)]
        public string PPDetailStatus { get; set; } = string.Empty;

        [MaxLength(20)]
        public string DeliveryMethod { get; set; } = string.Empty;

        [MaxLength(50)]
        public string MainPartsLinkNumber { get; set; } = string.Empty;

        [MaxLength(35)]
        public string PurchaseOrderNumber { get; set; } = string.Empty;

        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [NotMapped]
        public List<WarehousePartDTO> WarehouseLocations { get; set; } = new List<WarehousePartDTO>();

        #endregion
    }
}
