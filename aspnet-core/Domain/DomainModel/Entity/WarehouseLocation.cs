using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(WarehouseId), nameof(Location), nameof(Zoning), nameof(Height), nameof(Sequence), Name = "IDX_WAREHOUSELOCATION")]
    public class WarehouseLocation : BaseModel
    {
        #region Properties
        [ForeignKey("FK_WarehouseLocation_WarehouseId")]
        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        [MaxLength(15)]
        public string Location { get; set; } = string.Empty;
        public int Zoning { get; set; }
        public int Height { get; set; }
        public int Sequence { get; set; }

        [NotMapped]
        public int ProductId { get; set; }
        [NotMapped]
        public int Quantity { get; set; }
        #endregion
    }
}
