using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(VendorCode), nameof(VendorPartNumber), nameof(PartsLinkNumber), Name = "IDX_VENDORCATALOG")]
    public class VendorCatalog: BaseModel
    {
        #region Properties
        [MaxLength(25)]
        public string VendorCode { get; set; } = string.Empty;

        [MaxLength(25)]
        public string VendorPartNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string PartsLinkNumber { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int OnHand { get; set; }

        [NotMapped]
        public DateTime CutoffTime { get; set; }
        #endregion
    }
}
