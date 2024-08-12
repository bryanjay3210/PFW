using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class TotalSalesDTO
    {
        public decimal SalesAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal NetSalesAmount { get; set; }
        public decimal UnitCost { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal ReturnPercentage { get; set; }
        public string SalesPerson { get; set; } = string.Empty;
    }
}
