using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public int AccountNumber { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal? Discount { get; set; }
        public decimal? TaxRate { get; set; }
        public int? PaymentTermId { get; set; }
        public int? PriceLevelId { get; set; }

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
        public bool IsHoldAccount { get; set; }
        public decimal CreditLimit { get; set; }
        public bool IsBypassCreditLimit { get; set; }
        public decimal OverBalance { get; set; }

        public decimal StatementAmount { get; set; }
        //public bool HasValidContacts { get; set; }
        public List<Contact> Contacts { get; set; } = new List<Contact>();
        public CustomerCredit? CustomerCredit { get; set; }
        public List<Location>? Locations { get; set; } = new List<Location>();
    }
}
