using LBDIdentityServer4.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LBDIdentityServer4.Quickstart.Role
{
    public class RoleUserViewModel
    {
        public RoleUserViewModel()
        {
            users = new List<ApplicationUser>();
        }
        public string user_id { get; set; }
        
        public string role_id { get; set; }

        public List<ApplicationUser> users { get; set; }
    }
}
