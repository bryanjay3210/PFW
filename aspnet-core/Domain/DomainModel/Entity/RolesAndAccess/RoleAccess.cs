using Domain.DomainModel.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity.RolesAndAccess
{
    public class RoleAccess: BaseModel
    {
        #region Properties
        [ForeignKey("FK_RoleAccess_RoleId")]
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        [ForeignKey("FK_RoleAccess_AccessId")]
        public int AccessId { get; set; }
        public Access? Access { get; set; }

        public int AccessType { get; set; } = 0;
        #endregion
    }
}
