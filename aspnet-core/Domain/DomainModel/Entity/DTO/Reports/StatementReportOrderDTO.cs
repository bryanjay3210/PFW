using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class StatementReportOrderDTO
    {
        public int OrderId { get; set; }
        public int? OrderNumber { get; set;}
        public int? InvoiceNumber { get; set; }
        public string? PurchaseOrderNumber { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? Balance { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderType { get; set; } = string.Empty;
    }
}
