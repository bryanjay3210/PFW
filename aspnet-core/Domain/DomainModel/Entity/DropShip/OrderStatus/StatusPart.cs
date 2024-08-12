using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DropShip
{
    public class StatusPart
    {
        public string Status { get; set; } = string.Empty;
        public string? Tracking { get; set; } = string.Empty;
        public DateTime? ShipDate { get; set; }
        public string? Carrier { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal QuantityOrdered { get; set; }
        public decimal QuantityShipped { get; set; }
}
}
