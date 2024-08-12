using Domain.DomainModel.Interface;
using System.ComponentModel.DataAnnotations;

namespace Domain.DomainModel.Base
{
    public class AddressBaseModel : BaseModel, IAggregateRoot
    {
        #region Properties
        [MaxLength(500)]
        public string AddressLine1 { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? AddressLine2 { get; set; }

        [MaxLength(20)]
        public string City { get; set; } = string.Empty;

        [MaxLength(50)]
        public string State { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Country { get; set; } = string.Empty;

        [MaxLength(10)]
        public string ZipCode { get; set; } = string.Empty;

        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(15)]
        public string FaxNumber { get; set; } = string.Empty;

        [MaxLength(60)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Latitude { get; set; }

        [MaxLength(20)]
        public string? Longitude { get; set; }
        #endregion
    }
}
