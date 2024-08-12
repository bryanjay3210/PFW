using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(DriverLogId), nameof(OrderId), nameof(OrderDetailId), Name = "IDX_DRIVERLOGDETAIL")]
    public class DriverLogDetail : BaseModel
    {
        [ForeignKey("FK_DriverLogDetail_DriverLogId")]
        public int DriverLogId { get; set; }
        public DriverLog? DriverLog { get; set; }

        [ForeignKey("FK_DriverLogDetail_OrderId")]
        public int OrderId { get; set; }
        public int OrderNumber { get; set; }
        public Order? Order { get; set; }

        [ForeignKey("FK_DriverLogDetail_OrderDetailId")]
        public int OrderDetailId { get; set; }
        public OrderDetail? OrderDetail { get; set; }

        // Ship To Details
        [MaxLength(100)]
        public string ShipAddressName { get; set; } = string.Empty;

        public int OrderQuantity { get; set; }

        [MaxLength(20)]
        public string PartNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string PaymentTermName { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(12, 2)")]
        public decimal TotalAmount { get; set; }

        public int StatusId { get; set; }

        [MaxLength(100)]
        public string StatusDetail { get; set; } = string.Empty;

        [NotMapped]
        public decimal OrderTotalAmount { get; set; }
    }
}
