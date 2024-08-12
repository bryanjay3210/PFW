using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(PartNumber), nameof(Location), Name = "IDX_STOCKPARTSLOCATION")]
    public class StockPartsLocation: BaseModel
    {
        [MaxLength(20)]
        public string PartNumber { get; set; } = string.Empty;
        [MaxLength(15)]
        public string Location { get; set; } = string.Empty; // OSBIN
        [MaxLength(10)]
        public string? Area { get; set; }
        [MaxLength(10)]
        public string? WarehouseCode { get; set; } // WH_CODE
        public int Quantity { get; set; } // OSQTY
        public int? Status { get; set; }
        public int? LocationType { get; set; }
        public int? Pickable { get; set; }
        public int? Tag { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? Percentage { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? AccessDate { get; set; }
    }
}
