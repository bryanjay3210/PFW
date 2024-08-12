using Domain.DomainModel.Interface;
using System.ComponentModel.DataAnnotations;

namespace Domain.DomainModel.Base
{
    public class BaseModel : IAggregateRoot
    {
        private DateTime _createdDate;
        private DateTime? _modifiedDate;


        #region Properties
        public int Id { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public bool IsDeleted { get; set; } = false;
        
        [MaxLength(50)]
        public string CreatedBy { get; set; } = string.Empty;
        
        public DateTime CreatedDate 
        {
            get
            {
                return _createdDate.ToLocalTime();
            }
            set
            {
                _createdDate = value;
            }
        }

        [MaxLength(50)]
        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate 
        {
            get
            {
                return _modifiedDate.HasValue ? _modifiedDate.Value.ToLocalTime() : null;
            }
            set
            {
                _modifiedDate = value;
            }
        }
        #endregion
    }
}
