using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class SequenceDTO
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string? CatId { get; set; } = string.Empty;
        public string CategoryGroupDescription { get; set; } = string.Empty;
    }
}
