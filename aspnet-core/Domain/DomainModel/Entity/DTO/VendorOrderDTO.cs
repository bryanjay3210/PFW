using Domain.DomainModel.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class VendorOrderDTO : BaseModel
    {
        public int OrderId { get; set; }
        public int OrderDetailId { get; set; }
        public int OrderNumber { get; set; }
        public string PurchaseOrderNumber { get; set; } = string.Empty;
        public string MainPartsLinkNumber { get; set; } = string.Empty;
        public string DeliveryMethod { get; set; } = string.Empty;
        public string Sequence { get; set; } = string.Empty;
        public int OrderQuantity { get; set; }
        public string ShipZone { get; set; } = string.Empty;
        public string PartNumber { get; set; } = string.Empty;
        public string VendorPartNumber { get; set; } = string.Empty;
        public decimal VendorPrice { get; set; }
        public string PartDescription { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string PODetailStatus { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime DeliveryDate { get; set; }
        public int? DeliveryRoute { get; set; }
    }
}
