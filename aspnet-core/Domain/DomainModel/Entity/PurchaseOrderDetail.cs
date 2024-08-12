using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(PurchaseOrderId), nameof(OrderNumber), nameof(PartNumber), nameof(VendorPartNumber), nameof(OrderDate), nameof(ReceivedDate),
           Name = "IDX_PURCHASE_ORDER_DETAIL")]
    public class PurchaseOrderDetail : BaseModel
    {
        private DateTime _orderDate;
        private DateTime _deliveryDate;
        private DateTime? _receivedDate;

        #region Properties
        [ForeignKey("FK_PurchaseOrderDetail_PurchaseOrderId")]
        public int PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }

        [ForeignKey("FK_PurchaseOrderDetail_OrderDetailId")]
        public int OrderDetailId { get; set; }
        public OrderDetail? OrderDetail { get; set; }

        public int OrderId { get; set; }
        public int OrderNumber { get; set; }

        public int OrderQuantity { get; set; }

        public string ShipZone { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PartNumber { get; set; } = string.Empty;

        [MaxLength(25)]
        public string VendorPartNumber { get; set; } = string.Empty;

        public decimal VendorPrice { get; set; }

        [MaxLength(200)]
        public string Sequence { get; set; } = string.Empty;

        [MaxLength(200)]
        public string PartDescription { get; set; } = string.Empty;

        public DateTime OrderDate
        {
            get
            {
                return this._orderDate.ToLocalTime();
            }
            set
            {
                this._orderDate = value;
            }
        }

        public DateTime DeliveryDate
        {
            get
            {
                return this._deliveryDate.ToLocalTime();
            }
            set
            {
                this._deliveryDate = value;
            }
        }

        public int DeliveryRoute { get; set; }

        public int StatusId { get; set; }

        [MaxLength(20)]
        public string PODetailStatus { get; set; } = string.Empty;

        [MaxLength(20)]
        public string DeliveryMethod { get; set; } = string.Empty;

        [MaxLength(50)]
        public string MainPartsLinkNumber { get; set; } = string.Empty;

        [MaxLength(35)]
        public string PurchaseOrderNumber { get; set; } = string.Empty;

        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;

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
        #endregion
    }
}
