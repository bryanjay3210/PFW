using Domain.DomainModel.Base;
using Domain.DomainModel.Entity.RolesAndAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity
{
    public class User: BaseModel
    {
        #region Properties
        [ForeignKey("FK_User_RoleId")]
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        [ForeignKey("FK_User_CustomerId")]
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [ForeignKey("FK_User_LocationId")]
        public int? LocationId { get; set; }
        public Location? Location { get; set; }
                
        [MaxLength(200)]
        public string UserName { get; set; } = string.Empty;
        
        [MaxLength(60)]
        public string Email { get; set; } = string.Empty;
        
        public bool IsCustomerUser { get; set; } = false;
        
        public bool IsActivated { get; set; } = false;
        
        public bool IsChangePasswordOnLogin { get; set; } = true;
                
        public byte[] PasswordHash { get; set; }
                
        public byte[] PasswordSalt { get; set; }
        #endregion
    }
}
