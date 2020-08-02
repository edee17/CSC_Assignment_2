using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheLifeTimeTalents.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleID { get; set; }
        public Role Role { get; set; }
    }
}
