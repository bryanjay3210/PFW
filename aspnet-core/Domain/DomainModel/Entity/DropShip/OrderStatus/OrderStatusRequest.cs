using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DropShip
{
    public class OrderStatusRequest
    {
        public List<StatusOrder> Orders { get; set; } = new List<StatusOrder>();
        public DateTime TransactionDate { get; set; }
    }
}
