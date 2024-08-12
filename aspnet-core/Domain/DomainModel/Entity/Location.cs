using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using Domain.DomainModel.Entity.Lookup;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(CustomerId), Name = "IDX_LOCATIONCUSTOMER")]
    [Index(nameof(LocationCode), nameof(LocationName), nameof(AddressLine1), nameof(AddressLine2), nameof(City), nameof(Country), nameof(ZipCode), nameof(Latitude), nameof(Longitude), Name = "IDX_LOCATION")]
    [Index(nameof(PhoneNumber), nameof(FaxNumber), nameof(Email), Name = "IDX_LOCATIONCONTACT")]
    [Index(nameof(LocationTypeId), Name = "IDX_LOCATIONLOCATIONTYPE")]
    public class Location : AddressBaseModel
    {
        #region Properties
        [ForeignKey("FK_Location_CustomerId")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [ForeignKey("FK_Location_LocationTypeId")]
        public int LocationTypeId { get; set; }
        public LocationType? LocationType { get; set; }

        [MaxLength(100)]
        public string LocationName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string LocationCode { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Notes { get; set; }

        [NotMapped]
        public string Zone { get; set; } = string.Empty;
        #endregion
    }
}
