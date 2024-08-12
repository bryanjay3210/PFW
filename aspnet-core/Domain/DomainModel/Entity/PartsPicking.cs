using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(PickNumber), nameof(StatusId), nameof(PickStatus), Name = "IDX_PARTS_PICKING")]
    public class PartsPicking : BaseModel
    {
        private DateTime _partsPickingDate;

        #region Properties
        [MaxLength(25)]
        public string PickNumber { get; set; } = string.Empty;

        public int StatusId { get; set; }

        [MaxLength(100)]
        public string PickStatus { get; set; } = string.Empty;

        public DateTime PartsPickingDate
        {
            get
            {
                return _partsPickingDate.ToLocalTime();
            }
            set
            {
                _partsPickingDate = value;
            }
        }

        public bool IsPrinted { get; set; }

        [NotMapped]
        public List<PartsPickingDetail> PartsPickingDetails { get; set; } = new List<PartsPickingDetail>();
        #endregion
    }
}
