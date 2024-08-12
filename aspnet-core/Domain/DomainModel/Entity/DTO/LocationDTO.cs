using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class LocationDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int LocationTypeId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string LocationCode { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }

        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Zone { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string FaxNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
    }
}
