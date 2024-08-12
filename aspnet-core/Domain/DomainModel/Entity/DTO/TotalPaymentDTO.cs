using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class TotalPaymentDTO
    {
        public decimal CashAmount { get; set; }
        public decimal CheckAmount { get; set; }
        public decimal CreditCardAmount { get; set; }
        public decimal ACHAmount { get; set; }
        public decimal WriteOff { get; set; }
        public decimal CreditMemoAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
