using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class PaymentHistoryDTO
    {
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal TotalAmountDue { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal PaymentBalance { get; set; }
        public decimal CustomerCreditAmountUsed { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public string? ReferenceNumber { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal InvoicePaymentAmount { get; set; }
        public decimal InvoiceBalance { get; set;}
        public string? LinkedInvoiceNumber { get; set; }
    }
}
