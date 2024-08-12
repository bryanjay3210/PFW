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
    [Index(nameof(DriverId), nameof(DriverName), nameof(PartsManifestNumber), Name = "IDX_PARTSMANIFEST")]
    public class PartsManifest : BaseModel
    {
        [ForeignKey("FK_PartsManifest_DriverId")]
        public int DriverId { get; set; }
        public Driver? Driver { get; set; }

        [MaxLength(100)]
        public string DriverName { get; set; } = string.Empty;

        [MaxLength(10)]
        public string PartsManifestNumber { get; set; } = string.Empty;

        public int PurposeId { get; set; }
        [MaxLength(15)]
        public string PurposeName { get; set; } = string.Empty;

        public int? VendorId { get; set; }

        [MaxLength(25)]
        public string? VendorCode { get; set; }

        [MaxLength(200)]
        public string? VendorName { get; set; }

        [NotMapped]
        public List<PartsManifestDetail> PartsManifestDetails { get; set; } = new List<PartsManifestDetail>();
    }
}
