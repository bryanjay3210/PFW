using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(WarehouseName), nameof(Description), Name = "IDX_WAREHOUSE")]
    public class Warehouse : BaseModel
    {
        #region Properties
        [MaxLength(50)]
        public string WarehouseName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }
        #endregion
    }
}
