using Domain.DomainModel.Base;
using Domain.DomainModel.Entity.RolesAndAccess;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Domain.DomainModel.Entity
{
    [Index(nameof(CatId), nameof(Description), nameof(Type), nameof(EbayId), nameof(GroupTitle), nameof(GroupLine), nameof(ClassId), nameof(Est), Name = "IDX_CATEGORY")]
    public class Category: BaseModel
    {
        [MaxLength(10)]
        public string CatId { get; set; } = string.Empty;
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
        [MaxLength(50)]
        public int? Type { get; set; }
        [MaxLength(200)]
        public decimal? EbayId { get; set; }
        [MaxLength(50)]
        public string? GroupTitle { get; set; }
        [MaxLength(50)]
        public int? GroupLine { get; set; }
        [MaxLength(20)]
        public string? ClassId { get; set; }
        [MaxLength(50)]
        public string? Est { get; set; }

        [NotMapped]
        public List<Sequence> Sequences { get; set; } = new List<Sequence>();
    }
}
