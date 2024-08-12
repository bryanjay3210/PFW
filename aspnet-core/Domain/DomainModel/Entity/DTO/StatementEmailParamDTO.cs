using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class StatementEmailParamDTO
    {
        public DateTime ReportDate { get; set; }
        public DateTime DueDate { get; set; }
        public int PaymentTermId { get; set; }
        public List<int> CustomerIds { get; set; } = new List<int>();
    }
}
