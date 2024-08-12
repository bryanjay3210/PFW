using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class CustomerSalesAmountDTO
    {
        public int CustomerId { get; set; }
        public int AccountNumber { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public SalesData Column1 { get; set; } = new SalesData();
        public SalesData Column2 { get; set; } = new SalesData();
        public SalesData Column3 { get; set; } = new SalesData();
        public SalesData Column4 { get; set; } = new SalesData();
        public SalesData Column5 { get; set; } = new SalesData();
        public SalesData Column6 { get; set; } = new SalesData();
        public SalesData Column7 { get; set; } = new SalesData();
        public SalesData Column8 { get; set; } = new SalesData();
        public SalesData Column9 { get; set; } = new SalesData();
        public SalesData Column10 { get; set; } = new SalesData();
        public SalesData Column11 { get; set; } = new SalesData();
        public SalesData Column12 { get; set; } = new SalesData();

    }

    public class SalesData
    {
        public Decimal? Amount { get; set; } = Decimal.Zero;
        public Decimal? Cost { get; set; } = Decimal.Zero;
        public Decimal? Profit { get; set; } = Decimal.Zero;
        public Decimal? Margin { get; set; } = Decimal.Zero;
    }
}
