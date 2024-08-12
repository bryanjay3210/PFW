using Domain.DomainModel.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity
{
    public class PriceLevel : BaseModel
    {
        #region Properties
        [MaxLength(50)]
        public string LevelName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal Amount { get; set; }

        public int? MarkUp { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }
        #endregion
    }
}
