using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(PartNumber), nameof(PartDescription), nameof(Brand), nameof(PartSizeId), nameof(CategoryId), nameof(SequenceId), Name = "IDX_PRODUCT")]
    public class Product: BaseModel
    {
        #region Properties

        [MaxLength(20)]
        public string PartNumber { get; set; } = string.Empty;
        [NotMapped]
        public string OriginalPartNumber { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? PartDescription { get; set; }
        
        [MaxLength(200)]
        public string? Brand { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal? PriceLevel1 { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? PriceLevel2 { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? PriceLevel3 { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? PriceLevel4 { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? PriceLevel5 { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? PriceLevel6 { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? PriceLevel7 { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? PriceLevel8 { get; set; }
        
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? PreviousCost { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? CurrentCost { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? OEMListPrice { get; set; }

        [MaxLength(2000)]
        public string? ImageUrl { get; set; }
        public byte[]? Image { get; set; }

        public int? CategoryId { get; set; }
        public int? SequenceId { get; set; }
        public int? StatusId { get; set; }
        public int? PartSizeId { get; set; } // Small Medium Large
        public DateTime? DateAdded { get; set; }

        public int? OnReceivingHold { get; set; }
        public int? OnOrder { get; set; }
        public bool? IsDropShipAllowed { get; set; }
        public bool? IsWebsiteOption { get; set; }

        public bool IsCAProduct { get; set; } = false;
        public bool IsNVProduct { get; set; } = false;
        public bool IsAPIAllowed { get; set; } = false;

        [NotMapped]
        public string? PartsLinkNumber { get; set; }

        [NotMapped]
        public string? OEMNumber { get; set; }

        [NotMapped]
        public List<ItemMasterlistReference>? PartsLinkList { get; set; }

        [NotMapped]
        public string? PartsLinks { 
            get
            {
                return PartsLinkList != null && PartsLinkList.Count > 0 ? string.Join(',', PartsLinkList.Select(e => e.PartsLinkNumber)) : "";
            }
        }

        [NotMapped]
        public string? OEMs { 
            get 
            {
                return PartsLinkList != null && PartsLinkList.Count > 0 ? string.Join(',', PartsLinkList.Select(e => e.OEMNumber)) : "";
            } 
        }

        [NotMapped]
        public List<VendorCatalog>? VendorCatalogList { get; set; }

        [NotMapped]
        public string? VendorCodes
        {
            get
            {
                return VendorCatalogList != null && VendorCatalogList.Count > 0 ? string.Join(',', VendorCatalogList.Select(e => e.VendorCode)) : "";
            }
        }

        [NotMapped]
        public string? VendorPartNumbers
        {
            get
            {
                return VendorCatalogList != null && VendorCatalogList.Count > 0 ? string.Join(',', VendorCatalogList.Select(e => e.VendorPartNumber)) : "";
            }
        }

        [NotMapped]
        public int YearFrom { get; set; }

        [NotMapped]
        public int YearTo { get; set; }

        [NotMapped]
        public string? CopyType { get; set; }

        [NotMapped]
        public int? StockQuantity { get; set; }

        //public decimal? OEMListPrice { get; set; }
        //public int? AvailabilityId { get; set; }
        //MonthlyUnit    By month numbers Year to Date
        //Vendor  List of Vendors with other fields
        //Parts Application   list of Year make and model Etc
        #endregion
    }
}
