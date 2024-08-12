namespace Domain.DomainModel.Entity.DTO
{
    public class PrintInvoiceDTO
    {
        private DateTime _orderDate;
        private DateTime _deliveryDate;
        private DateTime? _transactionDate;
        private DateTime? _postDate;
        private DateTime? _paymentDate;


        #region Properties
        public int? OrderNumber { get; set; } // Can have zero if Quote. Should be updated when quote turned to Order
        public int? QuoteNumber { get; set; } // Can have zero if Order. Should be updated when Create Order is saved as Quote
        public int? InvoiceNumber { get; set; }
        public string? PurchaseOrderNumber { get; set; } = string.Empty;
        public string PFOSAddress { get; set; } = string.Empty;
        public string PFOSPhoneNumber { get; set; } = string.Empty; 
        public string PFOSWebsite { get; set; } = string.Empty;

        public string SoldTo { get; set; } = string.Empty;
        public string SoldToAddress { get; set; } = string.Empty;
        public string ShipTo { get; set; } = string.Empty;
        public string ShipToAddress { get; set; } = string.Empty;

        public int CustomerAccountNumber { get; set; }
        public string CustomerPhoneNumber { get; set; } = string.Empty;
        public string CustomerPaymentTerm{ get; set; } = string.Empty;

        public string SoldBy { get; set; } = string.Empty;
        public string OrderedByNotes { get; set; } = string.Empty;
        public string OrderedBy { get; set; } = string.Empty;
        public string OrderedByPhoneNumber { get; set; } = string.Empty;

        public DateTime DeliveryDate
        {
            get
            {
                return _deliveryDate.ToLocalTime();
            }
            set
            {
                _deliveryDate = value;
            }
        }
        public int? DeliveryRoute { get; set; }

        public decimal TotalTax { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsQuote { get; set; } = false;

        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();


        
        #endregion
    }
}
