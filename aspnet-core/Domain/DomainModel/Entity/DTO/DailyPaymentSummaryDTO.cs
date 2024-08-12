using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class DailyPaymentSummaryDTO
    {
        public TotalPaymentDTO CASummary { get; set; } = new TotalPaymentDTO();
        public TotalPaymentDTO NVSummary { get; set; } = new TotalPaymentDTO();
    }
}
