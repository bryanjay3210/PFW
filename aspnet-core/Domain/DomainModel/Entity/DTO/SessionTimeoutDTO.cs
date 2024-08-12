using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity.DTO
{
    public class SessionTimeoutDTO
    {
        public int SessionTimeout { get; set; }
        public int DialogCountdown { get; set; }
    }
}
