using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class OverpaymentParameterDTO
    {
        public int CustomerId { get; set; }
        //public int OriginalInvoiceNumber { get; set; }
        public decimal Amount { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
