using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LBDIdentityServer4.Quickstart.ConfigurationManage.ApiResource
{
    public class AddApiResourceModel
    {
        public string name { get; set; }



        public List<ApiResourceScore> scores { get; set; }
    }

    public class ApiResourceScore
    {
        public string name { get; set; }

        public string display_name { get; set; }

        public string description { get; set; }

    }

    public  class ApiClaims
    {
        public string type { get; set; }
    }

}
