using Domain.DomainModel.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity
{
    public class SalesRepresentative : BaseModel
    {
        #region Properties   
        [MaxLength(50)]
        public string ContactName { get; set; } = string.Empty;
                
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(60)]
        public string? Email { get; set; }

        public int Type { get; set; } = 1; /* 1 = In | 0 = Out */

        [MaxLength(2000)]
        public string? Notes { get; set; }
        #endregion
    }
}
