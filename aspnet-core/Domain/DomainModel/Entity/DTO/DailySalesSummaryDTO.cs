using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class DailySalesSummaryDTO
    {
        public decimal CATotalProfitMargin { get; set; }
        public decimal CATotalReturnPercentage { get; set; }
        public decimal NVTotalProfitMargin { get; set; }
        public decimal NVTotalReturnPercentage { get; set; }
        public List<TotalSalesDTO> CASummary { get; set; } = new List<TotalSalesDTO>();
        public List<TotalSalesDTO> NVSummary { get; set; } = new List<TotalSalesDTO>();
    }
}
