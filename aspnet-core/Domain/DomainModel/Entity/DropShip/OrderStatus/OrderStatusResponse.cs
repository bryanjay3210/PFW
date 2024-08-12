using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DropShip
{
    public class OrderStatusResponse
    {
        public List<StatusOrderParts> Orders { get; set; } = new List<StatusOrderParts>();
        public DateTime TransactionDate { get; set; }
    }
}
