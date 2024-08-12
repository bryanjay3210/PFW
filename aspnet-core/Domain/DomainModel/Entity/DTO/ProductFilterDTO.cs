using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class ProductFilterDTO
    {
        public int State { get; set; }
        public int? Year { get; set; }
        public List<int>? CategoryIds { get; set; }
        public List<int>? SequenceIds { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public bool IsCAProduct { get; set; } = false;
        public bool IsNVProduct { get; set; } = false;
    }
}
