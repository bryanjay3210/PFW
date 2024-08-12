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
    [Index(nameof(CustomerId), nameof(AccountNumber), nameof(PaymentDate), nameof(ReferenceNumber), Name = "IDX_PAYMENT")]
    public class Payment : BaseModel
    {
        private DateTime _paymentDate;
        private DateTime _lockedDate;


        [ForeignKey("FK_Payment_CustomerId")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int AccountNumber { get; set; }
        public int? SalesRepresentativeId { get; set; }
        public DateTime? PaymentDate
        {
            get
            {
                return _paymentDate.ToLocalTime();
            }
            set
            {
                _paymentDate = value.Value;
            }
        }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal? TotalAmountDue { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal? PaymentAmount { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal? PaymentBalance { get; set; }

        [MaxLength(15)]
        public string? PaymentType { get; set; }

        [MaxLength(50)]
        public string? ReferenceNumber { get; set; }
        
        [MaxLength(1500)]
        public string? AppliesTo { get; set; }

        // For Credit Memo Types, we save the invoice number(s) of the credit memo that was used.
        [MaxLength(500)]
        public string? LinkedInvoiceNumber { get; set; }

        public bool IsReversePayment { get; set; } = false;
        public bool IsUseCustomerCredit { get; set; } = false;

        [ForeignKey("FK_Payment_CustomerCreditId")]
        public int? CustomerCreditId { get; set; }
        public CustomerCredit? CustomerCredit { get; set; }
        
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? CustomerCreditAmountUsed { get; set; }

        [MaxLength(30)]
        public string? PurchaseOrderNumber { get; set; }

        [MaxLength(20)]
        public string? Oti { get; set; }

        [MaxLength(50)]
        public string? LockedBy { get; set; }
        public DateTime Lockeddate
        {
            get 
            { 
                return _lockedDate.ToLocalTime();
            }
            set
            {
                _lockedDate = value;
            }
        }
        

        [MaxLength(3)]
        public string? CurrencyCode { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? ExchangeDifference { get; set; }
        
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? InvoiceRate { get; set; }
        [MaxLength(6)]
        public string? PaymentCurrencyCode { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? PaymentRate { get; set; }
        [MaxLength(1)]
        public string? Deposit { get; set; }
        
        [MaxLength(10)]
        public string? CheckStatus { get; set; }
        [MaxLength(100)]
        public string? Particular { get; set; }
        [MaxLength(15)]
        public string? Phone { get; set; }
        public string? Notes { get; set; }

        [NotMapped]
        public List<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();

        [NotMapped]
        public List<Order>? CreditMemos { get; set; }
    }
}
