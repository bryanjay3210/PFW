using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Reflection.Emit;
using System.Xml.Linq;
namespace Domain.DomainModel.Entity.RolesAndAccess
{
    [Index(nameof(UserTypeId), Name = "IDX_ROLEUSERTYPE")]
    public class Role: LookupBaseModel
    {
        #region Properties
        [ForeignKey("FK_Role_UserTypeId")]
        public int UserTypeId { get; set; }
        public UserType? UserType { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        [NotMapped]
        public List<RolePermission> RolePermissions{ get; set; } = new List<RolePermission>();
        #endregion
    }
}
