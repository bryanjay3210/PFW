using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DropShip
{
    public class PlacePart
    {
        public int OrderItemId { get; set; }
        public int? WarrantyItemID { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string PartName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int? Stock { get; set; }
        public decimal? VendorPrice { get; set; }
        public decimal? VendorCorePrice { get; set; }
        public decimal? ShipCharge { get; set; }
        public decimal ListPrice { get; set; }
        public string? CrossReference { get; set; } = string.Empty;
        public bool? IsSpecial { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
