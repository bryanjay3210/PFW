using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(VendorName), nameof(VendorCode), nameof(ContactName), nameof(PhoneNumber), nameof(Email), 
           nameof(CAPercentage), nameof(CARank), nameof(NVPercentage), nameof(NVRank), Name = "IDX_VENDOR")]
    public class Vendor : AddressBaseModel
    {
        private DateTime _cutoffTime;

        #region Properties
        [MaxLength(200)]
        public string VendorName { get; set; } = string.Empty;

        [MaxLength(25)]
        public string VendorCode { get; set; } = string.Empty;

        [MaxLength(200)]
        public string ContactName { get; set; } = string.Empty;

        public bool IsCAVendor { get; set; } = false;
        public bool IsNVVendor { get; set; } = false;

        public int CARank { get; set; }
        public int NVRank { get; set; }
        public int CAPercentage { get; set; }
        public int NVPercentage { get; set; }

        public DateTime CutoffTime
        {
            get
            {
                return _cutoffTime.ToLocalTime();
            }
            set
            {
                _cutoffTime = value;
            }
        }
        #endregion
    }
}
