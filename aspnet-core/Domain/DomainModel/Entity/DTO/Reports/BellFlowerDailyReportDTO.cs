using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO.Reports
{
    public class BellFlowerDailyReportDTO
    {
        public string vendor_brand { get; set; } = string.Empty;
        public string vendor_brand_code { get; set; } = string.Empty;
        public string vendor_sku { get; set; } = string.Empty;
        public string vendor_sku_code { get; set; } = string.Empty;
        public decimal vendor_cost { get; set; } = 0;
        public decimal vendor_core_cost { get; set; } = 0;
        public int stock { get; set; }
        public string old_vendor_sku { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
    }
}
