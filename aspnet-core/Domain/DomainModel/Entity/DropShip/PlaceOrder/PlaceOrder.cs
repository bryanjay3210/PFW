using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DropShip
{
    public class PlaceOrder
    {
        public string VendedId { get; set; } = string.Empty;
        public string DeliveryName { get; set; } = string.Empty;
        public string DeliveryStreet { get; set; } = string.Empty;
        public string? DeliverySuburb { get; set; }
        public string DeliveryCity { get; set; } = string.Empty;
        public string DeliveryState { get; set; } = string.Empty;
        public string? DeliveryTelephone { get; set; }
        public string DeliveryPostCode { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public int? SupplierOrderId { get; set; }
        public string Carrier { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string? Comments { get; set; }
        public string ShippingMethod { get; set; } = string.Empty;
        public int? WOSCustomerId { get; set; }
        public int? ShipperAccountNumber { get; set; }
        public string? CrossReferencePO { get; set; } = string.Empty;
        public int EstFreight { get; set; }
        public string? AnError { get; set; } = string.Empty;
        public int? SONumber { get; set; }
    }
}
