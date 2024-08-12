using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity
{
    public class UserDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string Password { get; set; } = string.Empty;
        public bool IsCustomerUser { get; set; } = false;
        public int? CustomerId { get; set; }
        public int? LocationId { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsActivated { get; set; } = false;
        public bool IsChangePasswordOnLogin { get; set; } = true;
    }
}
