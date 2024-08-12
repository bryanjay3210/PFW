using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using Domain.DomainModel.Entity.Lookup;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    public class UserNotificationDTO
    {
        #region Properties
        public List<UserNotificationFollowUpDTO> FollowUpList { get; set; } = new List<UserNotificationFollowUpDTO>();
        public List<UserNotificationCreditNoteDTO> CreditNoteList { get; set; } = new List<UserNotificationCreditNoteDTO>();
        #endregion
    }
    
}
