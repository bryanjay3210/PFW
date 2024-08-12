using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using Domain.DomainModel.Entity.Lookup;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    public class UserNotificationFollowUpDTO
    {
        #region Properties
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        #endregion
    }
    
}
