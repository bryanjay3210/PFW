using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using Domain.DomainModel.Entity.Lookup;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(CustomerId), Name = "IDX_CONTACTCUSTOMER")]
    [Index(nameof(LocationId), Name = "IDX_CONTACTLOCATION")]
    [Index(nameof(PositionTypeId), Name = "IDX_CONTACTPOSITION")]
    [Index(nameof(ContactName), nameof(PhoneNumber), nameof(Email), Name = "IDX_CONTACT")]
    public class Contact : BaseModel
    {
        #region Properties
        [ForeignKey("FK_Contact_CustomerId")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [ForeignKey("FK_Contact_LocationId")]
        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        [ForeignKey("FK_Contact_PositionTypeId")]
        public int? PositionTypeId { get; set; }
        public PositionType? PositionType { get; set; }

        [MaxLength(50)]
        public string ContactName { get; set; } = string.Empty;
        
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(60)]
        public string? Email { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        public bool IsEmailOrder { get; set; } = false;
        public bool IsEmailCreditMemo { get; set; } = false;
        public bool IsEmailStatement { get; set; } = false;
        #endregion
    }
}
