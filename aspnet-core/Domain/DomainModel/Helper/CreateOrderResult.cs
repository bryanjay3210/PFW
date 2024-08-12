using Domain.DomainModel.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Helper
{
    public class CreateOrderResult
    {
        public Order Order { get; set; } = new Order();
        public int? OrderNumber { get; set; }
        public int? QuoteNumber { get; set; }
        public bool OrderResult { get; set; }
        public bool? SyncResult { get; set; }
        public bool IsQuote { get; set; }
    }
}
