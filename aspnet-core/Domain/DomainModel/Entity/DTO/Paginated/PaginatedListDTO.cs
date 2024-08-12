using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Domain.DomainModel.Entity.DTO.Paginated
{
    public class PaginatedListDTO<T>
    {
        public int RecordCount { get; set; }
        public List<T> Data { get; set; } = new List<T>();
    }
}
