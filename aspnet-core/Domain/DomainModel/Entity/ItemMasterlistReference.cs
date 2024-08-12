using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(ProductId), nameof(PartNumber), nameof(PartsLinkNumber), nameof(OEMNumber), Name = "IDX_ITEMMASTERLISTREFERENCE")]
    public class ItemMasterlistReference: BaseModel
    {
        #region Properties
        [ForeignKey("FK_ItemMasterlistReference_ProductId")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [MaxLength(20)]
        public string PartNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? PartsLinkNumber { get; set; }

        [MaxLength(50)]
        public string? OEMNumber { get; set; }

        public bool IsMainPartsLink { get; set; } = false;
        public bool IsMainOEM { get; set; } = false;
        #endregion
    }
}
