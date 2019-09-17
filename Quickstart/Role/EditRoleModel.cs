using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LBDIdentityServer4.Quickstart.Role
{
    public class EditRoleModel
    {
        public string id { get; set; }


        [Required]
        [Display(Name = "角色名称")]
        public  string name { get; set; }

        public List<string> users { get; set; }
    }
}
