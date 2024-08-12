using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class StatementTotalReportDTO
    {
        public int CustomerId { get; set; }
        public int AccountNumber { get; set; }
        public decimal TotalDue { get; set; }
    }
}
