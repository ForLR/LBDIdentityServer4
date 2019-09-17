using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LBDIdentityServer4.Quickstart.Role
{
    public class AddRole
    {
        [Required]
        [Display(Name = "角色名字")]
        public string role_name { get; set; }
    }
}
