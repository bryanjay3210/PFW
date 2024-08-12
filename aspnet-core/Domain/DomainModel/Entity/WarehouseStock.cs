using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(ProductId), nameof(WarehouseId), nameof(WarehouseLocationId), nameof(Quantity), Name = "IDX_WAREHOUSESTOCK")]
    public class WarehouseStock : BaseModel
    {
        #region Properties
        [ForeignKey("FK_WarehouseStock_ProductId")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [ForeignKey("FK_WarehouseStock_WarehouseId")]
        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        [ForeignKey("FK_WarehouseStock_WarehouseLocationId")]
        public int WarehouseLocationId { get; set; }
        public WarehouseLocation? WarehouseLocation { get; set; }

        public int Quantity { get; set; }

        [NotMapped]
        public string Location { get; set; } = string.Empty;

        [NotMapped]
        public decimal CurrentCost { get; set; }
        #endregion
    }
}
