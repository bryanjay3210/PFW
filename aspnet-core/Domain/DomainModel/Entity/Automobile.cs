using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(Name), nameof(Make), nameof(Model), nameof(Type), nameof(Year), Name = "IDX_AUTOMOBILE")]
    public class Automobile : BaseModel
    {
        #region Properties
        [MaxLength(20)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string Make { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string Model { get; set; } = string.Empty;
        
        [MaxLength(10)]
        public string Type { get; set; } = string.Empty;
        
        public int Year { get; set; } = 0;
        [MaxLength(2000)]
        public string Notes { get; set; } = string.Empty;
        #endregion
    }
}
