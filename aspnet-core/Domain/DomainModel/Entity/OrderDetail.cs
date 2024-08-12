using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(OrderId), nameof(SalesOrderNumber), nameof(OriginalInvoiceNumber), nameof(BuyOutOrder), nameof(PartNumber),
           nameof(Vehicle), nameof(CategoryId), nameof(TrackingNumber), nameof(IsCreditMemoCreated), nameof(RGAInspectedCode),
           nameof(RGAPartNumber), nameof(RGALocation), nameof(RGAState), Name = "IDX_ORDERDETAIL")]
    public class OrderDetail : BaseModel
    {
        private DateTime? _shipDate;

        #region Properties
        // Customer Details
        [ForeignKey("FK_OrderDetail_OrderId")]
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        [ForeignKey("FK_OrderDetail_ProductId")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int CategoryId { get; set; }
        public int OrderQuantity { get; set; }
        public int? OnHandQuantity { get; set; }
        [MaxLength(15)]
        public string Location { get; set; } = string.Empty;
        public int? WarehouseLocationId { get; set; }

        [MaxLength(20)]
        public string PartNumber { get; set; } = string.Empty;
        [MaxLength(200)]
        public string PartDescription { get; set; } = string.Empty;
        [MaxLength(50)]
        public string Brand { get; set; } = string.Empty;
        [MaxLength(50)]
        public string MainPartsLinkNumber { get; set; } = string.Empty;
        [MaxLength(50)]
        public string MainOEMNumber { get; set; } = string.Empty;
        [MaxLength(25)]
        public string VendorCode { get; set; } = string.Empty;
        [MaxLength(25)]
        public string VendorPartNumber { get; set; } = string.Empty;
        [Column(TypeName = "decimal(12, 2)")]
        public decimal VendorPrice { get; set; }
        public int VendorOnHand { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal ListPrice { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal WholesalePrice { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal DiscountedPrice { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal? RestockingFee { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? RestockingAmount { get; set; }

        public decimal Discount { get; set; } = 0;

        [Column(TypeName = "decimal(12, 2)")]
        public decimal DiscountAmount { get; set; } = 0;

        public int DiscountRecord { get; set; } = 0;

        [Column(TypeName = "decimal(12, 2)")]
        public decimal DiscountRecordAmount { get; set; } = 0;

        [MaxLength(500)]
        public string? PartsLinks { get; set; }
        [MaxLength(500)]
        public string? OEMs { get; set; }
        [MaxLength(500)]
        public string? VendorCodes { get; set; }

        [NotMapped]
        public string? ImageUrl { get; set; }

        [NotMapped]
        public List<WarehouseStock>? Stocks { get; set; }

        [NotMapped]
        public List<VendorCatalog>? VendorCatalogs { get; set; }

        [NotMapped]
        public List<WarehouseTracking>? WarehouseTrackings { get; set; }

        [MaxLength(50)]
        public string? Vehicle { get; set; } = string.Empty;
        [MaxLength(50)]
        public string? SalesRepresentative { get; set; } = string.Empty;
        [MaxLength(20)]
        public string? WarehouseLocation { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string? VendorInfo { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? WarehouseTracking { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? Carrier { get; set; }
        [MaxLength(30)]
        public string? TrackingNumber { get; set; }

        public int? SalesOrderNumber { get; set; }
        public int? OriginalInvoiceNumber { get; set; }
        public int? BuyOutOrder { get; set; }
        public int? PartSize { get; set; }
        public int? StatusId { get; set; }
        public int? ShippedQuantity { get; set; }
        public int? ScannedQuantity { get; set; }
        public int? PickedQuantity { get; set; }
        
        public int? ReturnQuantity { get; set; }
        [NotMapped]
        public int? RemainingQuantity { get; set; }

        public int? ItemCatalog { get; set; }
        public int? ToShip { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal? Price { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? UnitCost { get; set; }
        [Column(TypeName = "decimal(12, 2)")]

        public decimal? ShipCharge { get; set; }
        public DateTime? ShipDate 
        {
            get
            {
                return _shipDate.HasValue ? _shipDate.Value.ToLocalTime() : null;
            }
            set
            {
                _shipDate = value;
            }
        }

        public int? RGAInspectedCode { get; set; }
        public int? RGAState { get; set; }
        [MaxLength(15)]
        public string? RGALocation { get; set; }
        [MaxLength(20)]
        public string? RGAPartNumber { get; set; }
        public bool IsCreditMemoCreated { get; set; } = false;
        #endregion
    }
}
