using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(VendorId), nameof(VendorCode), nameof(VendorPO), nameof(PFWBNumber), nameof(ReceivedDate), Name = "IDX_PURCHASE_ORDER")]
    public class PurchaseOrder : BaseModel
    {
        private DateTime _purchaseOrderDate;
        private DateTime? _receivedDate;

        #region Properties
        [ForeignKey("FK_PurchaseOrder_VendorId")]
        public int VendorId { get; set; }
        public Vendor? Vendor { get; set; }

        [MaxLength(500)]
        public string VendorName { get; set; } = string.Empty;

        [MaxLength(25)]
        public string VendorCode { get; set; } = string.Empty;

        [MaxLength(25)]
        public string? VendorPO { get; set; } = string.Empty;

        [MaxLength(25)]
        public string PFWBNumber { get; set; } = string.Empty;

        public int StatusId { get; set; }

        [MaxLength(20)]
        public string POStatus { get; set; } = string.Empty;

        //public DateTime PurchaseOrderDate { get; set; }
        public DateTime PurchaseOrderDate
        {
            get
            {
                return _purchaseOrderDate.ToLocalTime();
            }
            set
            {
                _purchaseOrderDate = value;
            }
        }

        public DateTime? ReceivedDate
        {
            get
            {
                return this._receivedDate != null ? this._receivedDate.Value.ToLocalTime() : null;
            }
            set
            {
                this._receivedDate = value;
            }
        }

        public bool IsPrinted { get; set; }

        [NotMapped]
        public int TotalQuantity { get; set; }
        
        [NotMapped]
        public decimal TotalAmount { get; set; }

        [NotMapped]
        public List<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
        #endregion
    }
}
