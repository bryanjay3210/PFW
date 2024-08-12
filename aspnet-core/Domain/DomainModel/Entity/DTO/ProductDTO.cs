using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string PartNumber { get; set; } = string.Empty;
        public string? PartDescription { get; set; }
        public string? Brand { get; set; }
        public int? CategoryId { get; set; }
        public int? SequenceId { get; set; }
        public int? StatusId { get; set; }
        public int? PartSizeId { get; set; } // Small Medium Large

        public string? PartsLinks { get; set; }
        public string? OEMs { get; set; }
        public string? VendorCodes { get; set; }
        public string? PartsLinkNumber { get; set; }
        public string? OEMNumber { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }

        public decimal? ListPrice { get; set; }
        public decimal? PriceLevel1 { get; set; }
        public decimal? PriceLevel2 { get; set; }
        public decimal? PriceLevel3 { get; set; }
        public decimal? PriceLevel4 { get; set; }
        public decimal? PriceLevel5 { get; set; }
        public decimal? PriceLevel6 { get; set; }
        public decimal? PriceLevel7 { get; set; }
        public decimal? PriceLevel8 { get; set; }

        public int Stock { get; set; } = 0;
        public decimal CurrentCost { get; set; }
        public List<WarehouseStock>? Stocks { get; set; }
        public List<VendorCatalog>? VendorCatalogs { get; set; }
    }
}
