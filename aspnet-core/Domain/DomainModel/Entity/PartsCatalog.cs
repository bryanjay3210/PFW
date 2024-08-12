using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(PartNumber), nameof(ProductId), nameof(Make), nameof(Model), nameof(YearFrom), nameof(YearTo), nameof(Cylinder), nameof(Liter), nameof(Brand), Name = "IDX_PARTS_CATALOG")]
    public class PartsCatalog: BaseModel
    {
        #region Properties
        [MaxLength(20)]
        public string PartNumber { get; set; } = string.Empty;

        [ForeignKey("FK_PartsCatalog_ProductId")]
        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        [MaxLength(20)]
        public string Make { get; set; } = string.Empty;
        [MaxLength(30)]
        public string Model { get; set; } = string.Empty;

        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public int Cylinder { get; set; }
        public decimal Liter { get; set; }

        [MaxLength(30)]
        public string? GroupHead { get; set; }
        [MaxLength(30)]
        public string? SubGroup { get; set; }
        [MaxLength(30)]
        public string? SubModel { get; set; }
        [MaxLength(100)]
        public string? Position { get; set; }
        [MaxLength(50)]
        public string? Brand { get; set; }
        [MaxLength(2000)]
        public string? Notes { get; set; }
        
        #endregion
    }
}
