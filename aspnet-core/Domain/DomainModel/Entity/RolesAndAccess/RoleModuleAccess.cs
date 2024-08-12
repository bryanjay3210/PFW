using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.DomainModel.Entity.RolesAndAccess
{
    [Index(nameof(RoleId), Name = "IDX_ROLEMODULEACCESSROLEID")]
    [Index(nameof(ModuleId), Name = "IDX_ROLEMODULEACCESSMODULEID")]
    [Index(nameof(AccessTypeId), Name = "IDX_ROLEMODULEACCESSACCESSTYPEID")]
    public class RoleModuleAccess: BaseModel
    {
        #region Properties
        [ForeignKey("FK_RoleModuleAccess_RoleId")]
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        [ForeignKey("FK_RoleModuleAccess_ModuleId")]
        public int ModuleId { get; set; }
        public Module? Module { get; set; }

        [ForeignKey("FK_RoleModuleAccess_AccessTypeId")]
        public int AccessTypeId { get; set; }
        public AccessType? AccessType { get; set; }
        #endregion
    }
}
