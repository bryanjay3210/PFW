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
    public class ProductListDTO
    {
        public int Id { get; set; }
        public string PartNumber { get; set; } = string.Empty;
        public string? PartDescription { get; set; }
        public string? ImageUrl { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public bool IsActive { get; set; }
        public bool IsCAProduct { get; set; } = false;
        public bool IsNVProduct { get; set; } = false;
    }
}
