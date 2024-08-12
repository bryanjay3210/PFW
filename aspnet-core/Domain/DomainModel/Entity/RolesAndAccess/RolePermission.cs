using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Reflection.Emit;
using System.Xml.Linq;
namespace Domain.DomainModel.Entity.RolesAndAccess
{
    public class RolePermission: BaseModel
    {
        #region Properties
        [ForeignKey("FK_RolePermission_RoleId")]
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        [ForeignKey("FK_RolePermission_ModuleGroupId")]
        public int ModuleGroupId { get; set; }
        public ModuleGroup? ModuleGroup { get; set; }

        [ForeignKey("FK_RolePermission_ModuleId")]
        public int ModuleId { get; set; }
        public Module? Module { get; set; }

        [ForeignKey("FK_RolePermission_AccessTypeId")]
        public int AccessTypeId { get; set; }
        public AccessType? AccessType { get; set; }
        #endregion
    }
}
