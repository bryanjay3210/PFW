using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(PartsManifestId), nameof(ProductId), nameof(PartNumber), Name = "IDX_PARTSMANIFESTDETAIL")]
    public class PartsManifestDetail : BaseModel
    {
        [ForeignKey("FK_PartsManifestDetail_PartsManifestId")]
        public int PartsManifestId { get; set; }
        public PartsManifest? PartsManifest { get; set; }

        [ForeignKey("FK_PartsManifestDetail_ProductId")]
        public int ProductId { get; set; }
        [MaxLength(20)]
        public string PartNumber { get; set; } = string.Empty;
        [MaxLength(200)]
        public string PartDescription { get; set; } = string.Empty;
        public Product? Product { get; set; }
        public int Quantity { get; set; }

    }
}
