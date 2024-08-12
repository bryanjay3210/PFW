using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Helper
{
    public partial class SyncOrderResult
    {
        public int Value { get; set; }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
