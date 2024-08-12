using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(CustomerId), Name = "IDX_CUSTOMERNOTECUSTOMER")]
    public class CustomerNote : BaseModel
    {
        #region Properties
        [ForeignKey("FK_CustomerNote_CustomerId")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        [MaxLength(2000)]
        public string Notes { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string? Comment { get; set; }
        #endregion
    }
}
