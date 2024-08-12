using Domain.DomainModel.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity.RolesAndAccess
{
    public class Module: LookupBaseModel
    {
        #region Properties
        [ForeignKey("FK_Module_ModuleGroupId")]
        public int ModuleGroupId { get; set; }
        public ModuleGroup? ModuleGroup { get; set; }
        
        [NotMapped]
        public int? AccessTypeId { get; set; }
        #endregion
    }
}
