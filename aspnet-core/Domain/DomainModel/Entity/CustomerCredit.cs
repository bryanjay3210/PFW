using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(CustomerId), nameof(ReferenceId), nameof(CreditType), Name = "IDX_PAYMENT")]
    public class CustomerCredit : BaseModel
    {
        [ForeignKey("FK_CustomerCredit_CustomerId")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public int? ReferenceId { get; set; } // PaymentId / ReturnId / 10001 - Discount / 
        
        public int CreditType { get; set; } // 1 - Payment / 2 - Return / 3 - Manual / 4 - Discount / 5 - Others

        public string CreditReason { get; set; } = string.Empty;

        public int PreviousRecordId { get; set; }
        public DateTime AmountPostedDate { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal CurrentBalance { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal PreviousBalance { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal AmountPosted { get; set; }
    }
}
