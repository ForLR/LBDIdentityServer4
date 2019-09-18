using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LBDIdentityServer4.Quickstart.ConfigurationManage
{
    public class ClientsViewModel
    {
        [Required]
        public  string client_id { get; set; }
        [Required]
        public string client_name { get; set; }

        public string client_secrets { get; set; }
        public string redirect_uris { get; set; }

        public string post_logout_redirect_uris { get; set; }

        public List<string> allowed_cors_origins { get; set; }

        [Required]
        public List<string> allowed_grant_types { get; set; }

        [Required]
        public List<string> allowed_scopes { get; set; }
    }
}
