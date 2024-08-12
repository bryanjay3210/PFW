using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class DeliverySummaryDTO
    {
        public DateTime DeliveryDate { get; set; }
        public DeliverySummaryCA DeliverySummaryCA { get; set; } = new DeliverySummaryCA();
        public DeliverySummaryNV DeliverySummaryNV { get; set; } = new DeliverySummaryNV();
    }

    public class DeliverySummaryCA
    {
        public int NumberOfStops { get; set; }
        public List<ZoneSummary> ZoneSummaries { get; set; } = new List<ZoneSummary>();
    }

    public class DeliverySummaryNV
    {
        public int NumberOfStops { get; set; }
        public List<ZoneSummary> ZoneSummaries { get; set; } = new List<ZoneSummary>();
    }

    public class ZoneSummary
    {
        public int Zone { get; set; }
        public int NumberOfStops { get; set; }
    }
}
