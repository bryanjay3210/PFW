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
    [Index(nameof(DriverId), nameof(DriverName), nameof(DriverLogNumber), Name = "IDX_DRIVERLOG")]
    public class DriverLog : BaseModel
    {
        [ForeignKey("FK_DriverLog_DriverId")]
        public int DriverId { get; set; }
        public Driver? Driver { get; set; }

        [MaxLength(100)]
        public string DriverName { get; set; } = string.Empty;

        [MaxLength(10)]
        public string DriverLogNumber { get; set; } = string.Empty;

        public int StatusId { get; set; }

        [MaxLength(100)]
        public string StatusDetail { get; set; } = string.Empty;

        [NotMapped]
        public List<DriverLogDetail> DriverLogDetails { get; set; } = new List<DriverLogDetail>();
    }
}
