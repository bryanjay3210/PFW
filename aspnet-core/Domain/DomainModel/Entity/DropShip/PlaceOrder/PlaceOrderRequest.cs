using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DropShip
{
    public class PlaceOrderRequest
    {
        public List<OrderInfo> OrderInfos { get; set; } = new List<OrderInfo>();
        public DateTime TransactionDate { get; set; }
    }
}
