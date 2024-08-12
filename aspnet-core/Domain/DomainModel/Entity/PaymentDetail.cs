using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(PaymentId), nameof(OrderNumber), nameof(InvoiceNumber), Name = "IDX_PAYMENT_DETAIL")]
    public class PaymentDetail : BaseModel
    {
        [ForeignKey("FK_PaymentDetail_PaymentId")]
        public int PaymentId { get; set; }
        public Payment? Payment { get; set; }

        public int OrderNumber { get; set; }
        public int InvoiceNumber { get; set; }
        
        [Column(TypeName = "decimal(12, 2)")]
        public decimal InvoiceAmount { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal PaymentAmount { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal InvoiceBalance { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? CustomerCreditAmountUsed { get; set; }

        // For Credit Memo Types, we save the invoice number(s) of the credit memo that was used.
        [MaxLength(500)]
        public string? LinkedInvoiceNumber { get; set; }
    }
}
