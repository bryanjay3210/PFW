using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class UserNotificationEmailDTO
    {
        public int OrderNumber { get; set; }
        public string PartNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
