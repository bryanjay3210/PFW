using Domain.DomainModel.Base;
using Domain.DomainModel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DropShip
{
    public class PartInquiry : IAggregateRoot
    {
        public int OrderItemId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Brand { get; set;} = string.Empty;
        public int Quantity { get; set; }
        public int? Stock { get; set; }
        public decimal VendorPrice { get; set; }
        public decimal? VendorCorePrice { get; set; }
        public decimal? ShipCharge { get; set; }
        public bool IsSpecial { get; set; }
        public string? Remarks { get; set; } = string.Empty;
    }
}
