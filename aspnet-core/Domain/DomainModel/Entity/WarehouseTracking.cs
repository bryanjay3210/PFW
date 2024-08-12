using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(OrderId), nameof(OrderDetailId), nameof(Status), nameof(Description), Name = "IDX_WAREHOUSE_TRACKING")]
    public class WarehouseTracking : BaseModel
    {
        #region Properties
        [ForeignKey("FK_WarehouseTracking_OrderId")]
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        [ForeignKey("FK_WarehouseTracking_OrderDetailId")]
        public int OrderDetailId { get; set; }
        public OrderDetail? OrderDetail { get; set; }


        [MaxLength(100)]
        public string Status { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }
        #endregion
    }
}
