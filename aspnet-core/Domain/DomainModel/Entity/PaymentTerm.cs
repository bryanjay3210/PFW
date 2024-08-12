using Domain.DomainModel.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity
{
    public class PaymentTerm : BaseModel
    {
        #region Properties
        [MaxLength(50)]
        public string TermName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        public int NumberOfDays { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }
        #endregion
    }
}
