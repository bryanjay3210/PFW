using Domain.DomainModel.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.DomainModel.Entity.RolesAndAccess
{
    public class Access: BaseModel
    {
        #region Properties
        [ForeignKey("FK_Access_ModuleId")]
        public int ModuleId { get; set; }
        public Module? Module { get; set; }
        
        public ICollection<RoleAccess>? RoleAccesses { get; set; }
        #endregion
    }
}
