using Domain.DomainModel.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(CategoryId), nameof(CatId), nameof(CategoryGroupDescription), nameof(Arrange), Name = "IDX_CATEGORY")]
    public class Sequence: BaseModel
    {
        #region Properties
        [ForeignKey("FK_Sequence_CategoryId")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [MaxLength(10)]
        public string? CatId { get; set; }
        [MaxLength(200)]
        public string CategoryGroupDescription { get; set; } = string.Empty;
        public decimal Arrange { get; set; }
        [MaxLength(200)]
        public string? Name { get; set; }
        [MaxLength(200)]
        public string? CategoryDescription2 { get; set; }
        [MaxLength(100)]
        public string? EstReference { get; set; }
        public decimal? Arr { get; set; }
        #endregion
    }
}
