using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(CustomerId), nameof(AccountNumber), nameof(OrderNumber), nameof(InvoiceNumber), nameof(WarehouseId), nameof(PurchaseOrderNumber),
           nameof(RGAType), nameof(RGAReason), Name = "IDX_ORDER")]
    
    public class Order : BaseModel
    {
        private DateTime _orderDate;
        private DateTime _deliveryDate;
        private DateTime? _transactionDate;
        private DateTime? _postDate;
        private DateTime? _paymentDate;

        #region Properties
        // Customer Details
        [ForeignKey("FK_Order_CustomerId")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [ForeignKey("FK_Order_PaymentTermId")]
        public int? PaymentTermId { get; set; }
        public PaymentTerm? PaymentTerm { get; set; }

        [ForeignKey("FK_Order_WarehouseId")]
        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        public int? OrderNumber { get; set; } // Can have zero if Quote. Should be updated when quote turned to Order
        public int? QuoteNumber { get; set; } // Can have zero if Order. Should be updated when Create Order is saved as Quote

        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;
        public int AccountNumber { get; set; }
        [MaxLength(50)]
        public string PaymentTermName { get; set; } = string.Empty;
        public int PriceLevelId { get; set; }
        [MaxLength(50)]
        public string PriceLevelName { get; set; } = string.Empty;
        [MaxLength(200)]
        public string WarehouseName { get; set; } = string.Empty;

        public DateTime OrderDate // Treated as Quote Date if Create Order is saved as Quote. Should be updated if previous quote was turned into Order
        {
            get
            {
                return _orderDate.ToLocalTime();
            }
            set
            {
                _orderDate = value.ToUniversalTime();
            }
        }

        public string User { get; set; } = string.Empty;
        public int OrderStatusId { get; set; }
        [MaxLength(50)]
        public string OrderStatusName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string OrderedBy { get; set; } = string.Empty;
        [MaxLength(60)]
        public string? OrderedByEmail { get; set; }
        [MaxLength(15)]
        public string? OrderedByPhoneNumber { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string? OrderedByNotes { get; set; } = string.Empty;
        public int DeliveryType{ get; set; }
        public DateTime DeliveryDate 
        {
            get
            {
                return _deliveryDate.ToLocalTime();
            }
            set
            {
                _deliveryDate = value.ToUniversalTime();
            }
        }
        public int? DeliveryRoute { get; set; }


        [Column(TypeName = "decimal(12, 2)")]
        public decimal? Discount { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal? RestockingFee { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? RestockingAmount { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? TaxRate { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? TotalTax { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal? SubTotalAmount { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? TotalAmount { get; set; }
        public bool IsQuote { get; set; } = false;
        public bool IsPrinted { get; set; } = false;

        public bool IsCustomerOrder { get; set; } = false;

        // For Order Types, we save the invoice number(s) of the credit memo that was used.
        // For Credit Memo Types, we save the invoice numbers where the credit memo was used.
        [MaxLength(500)]
        public string? LinkedInvoiceNumber { get; set; } 

        [NotMapped]
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        [MaxLength(60)]
        public string? Email { get; set; }
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        // Bill To Details
        [MaxLength(500)]
        public string BillAddress { get; set; } = string.Empty;
        [MaxLength(20)]
        public string BillCity { get; set; } = string.Empty;
        [MaxLength(50)]
        public string BillState { get; set; } = string.Empty;
        [MaxLength(10)]
        public string BillZipCode { get; set; } = string.Empty;
        [MaxLength(10)]
        public string BillZone { get; set; } = string.Empty;
        [MaxLength(15)]
        public string BillPhoneNumber { get; set; } = string.Empty;
        [MaxLength(50)]
        public string BillContactName { get; set; } = string.Empty;

        // Ship To Details
        [MaxLength(100)]
        public string ShipAddressName { get; set; } = string.Empty;
        [MaxLength(500)]
        public string ShipAddress { get; set; } = string.Empty;
        [MaxLength(20)]
        public string ShipCity { get; set; } = string.Empty;
        [MaxLength(50)]
        public string ShipState { get; set; } = string.Empty;
        [MaxLength(10)]
        public string ShipZipCode { get; set; } = string.Empty;
        [MaxLength(10)]
        public string ShipZone { get; set; } = string.Empty;
        [MaxLength(15)]
        public string ShipPhoneNumber { get; set; } = string.Empty;
        [MaxLength(50)]
        public string ShipContactName { get; set; } = string.Empty;

        // Order Details
        public bool IsHoldInvoice { get; set; } = false;
        public int? InvoiceNumber { get; set; }
        public int? Fwd { get; set; }
        public int? OriginalInvoiceNumber { get; set; }
        public int? OrderType { get; set; }
        public int? HandlingType { get; set; }
        public int? CreditType { get; set; }
        public int? SingleInvoice { get; set; }

        [MaxLength(35)]
        public string PurchaseOrderNumber { get; set; } = string.Empty;
        [MaxLength(10)]
        public string? Type { get; set; }
        [MaxLength(31)]
        public string? Category { get; set; }
        [MaxLength(30)]
        public string? Carrier { get; set; }
        [MaxLength(30)]
        public string? TrackingNumber { get; set; }

        [MaxLength(50)]
        public string? SalesRepresentative1 { get; set; } = string.Empty;
        [MaxLength(50)]
        public string? SalesRepresentative2 { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string? InvoiceNotes { get; set; } = string.Empty;

        [MaxLength(40)]
        public string? Particular { get; set; } = string.Empty;
        [MaxLength(10)]
        public string? PaymentReference { get; set; } = string.Empty;
        [MaxLength(50)]
        public string? PostedBy { get; set; } = string.Empty;
        [MaxLength(50)]
        public string? AccountingPaidBy { get; set; } = string.Empty;


        public DateTime? TransactionDate 
        {
            get
            {
                return _transactionDate.HasValue ? _transactionDate.Value.ToLocalTime() : null;
            }
            set
            {
               _transactionDate = value;
            }
        }
        public DateTime? PostDate 
        {
            get
            {
                return _postDate.HasValue ? _postDate.Value.ToLocalTime() : null;
            }
            set
            {
                _postDate = value;
            }
        }
        public DateTime? PaymentDate 
        {
            get
            {
                return _paymentDate.HasValue ? _paymentDate.Value.ToLocalTime() : null;
            }
            set
            {
                _paymentDate = value;
            }
        }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal? Debit { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? Credit { get; set; }
        public int? Quantity { get; set; }
        public int? QuantityShipped { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal? CurrentCost { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? AmountPaid { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? Balance { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? Payment { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? ListPrice { get; set; }

        //RGA
        public int RGAType { get; set; }
        public int RGAReason { get; set; }
        [MaxLength(500)]
        public string RGAReasonNotes { get; set; } = string.Empty;
        #endregion
    }
}
