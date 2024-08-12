using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class AgingBalanceReportDTO
    {
        public int CustomerId { get; set; }
        public int AccountNumber { get; set; }
        //public decimal TotalDue { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int PaymentTermId { get; set; }
        public string PaymentTermName { get; set;} = string.Empty;
        //public string AddressLine1 { get; set;} = string.Empty;
        //public string AddressLine2 { get; set; } = string.Empty;
        //public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        //public string Country { get; set; } = string.Empty;
        //public string ZipCode { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal? Current30Days { get; set; }
        public decimal? Over30Days { get; set; }
        public decimal? Over60Days { get; set; }
        public decimal? Over90Days { get; set; }
        public decimal? TotalOwed { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}
