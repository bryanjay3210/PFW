using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class TotalVendorSalesDTO
    {
        public string VendorName { get; set; } = string.Empty;
        public string VendorCode { get; set; } = string.Empty;
        public decimal SalesAmount { get; set; }
        public int Quantity { get; set; }
    }
}
