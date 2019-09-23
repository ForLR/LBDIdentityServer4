using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LBDIdentityServer4.Quickstart.ConfigurationManage.ApiResource
{
    public class ApiResourceViewModel
    {
        public string name { get; set; }

        public DateTime create_time { get; set; }


        public bool enabled { get; set; }

        public List<string> scope { get; set; }

    }
}
