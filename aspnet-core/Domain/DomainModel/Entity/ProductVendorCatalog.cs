using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity
{
    public class ProductVendorCatalog
    {
        [NotMapped]
        public VendorCatalog VendorCatalog { get; set; } = new VendorCatalog();

        [NotMapped]
        public List<string> PartsLinkNumbers { get; set; } = new List<string>(); 
    }
}
