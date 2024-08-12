using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainModel.Entity
{
    public class RegistrationResponse
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public User? User { get; set; } = new User();
        public List<User> Users { get; set; } = new List<User>();
    }
}
