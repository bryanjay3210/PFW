using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DropShip
{
    public class CheckInventoryRequest
    {
        public List<PartInquiry> Parts { get; set; } = new List<PartInquiry>();
        public DateTime TransactionDate { get; set; }
    }
}
