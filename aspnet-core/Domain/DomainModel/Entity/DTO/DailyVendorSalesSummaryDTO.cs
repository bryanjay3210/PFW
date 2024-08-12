using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class DailyVendorSalesSummaryDTO
    {
        public List<TotalVendorSalesDTO> VendorSummary { get; set; } = new List<TotalVendorSalesDTO>();
    }
}
