using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(OrderId), Name = "IDX_ORDERNOTEORDER")]
    public class OrderNote : BaseModel
    {
        #region Properties
        [ForeignKey("FK_OrderNote_OrderId")]
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        [MaxLength(2000)]
        public string Notes { get; set; } = string.Empty;
        #endregion
    }
}
