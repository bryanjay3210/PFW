using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class CustomerEmailDTO
    {
        public int CustomerId { get; set; }
        public int LocationId { get; set; }
        public int PositionTypeId { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
