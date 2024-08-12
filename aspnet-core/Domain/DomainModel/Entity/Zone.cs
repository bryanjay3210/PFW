using Domain.DomainModel.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity
{
    public class Zone : BaseModel
    {
        public int ZoneId { get; set; }
        [MaxLength(10)]
        public string BinCode { get; set; } = string.Empty;
        [MaxLength(10)]
        public string ZipCode { get; set; } = string.Empty;
    }
}
