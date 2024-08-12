using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using Domain.DomainModel.Entity.Lookup;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    public class UserNotificationCreditNoteDTO
    {
        #region Properties
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int? CreditMemoNumber { get; set; }
        public decimal? Amount { get; set; }
        public int OrderId { get; set; }
        #endregion
    }
    
}
