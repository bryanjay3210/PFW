using Domain.DomainModel.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity.RolesAndAccess
{
    public class ModuleGroup: LookupBaseModel
    {
        [NotMapped]
        public virtual List<Module> Modules { get; set; } = new List<Module>();
    }
}
