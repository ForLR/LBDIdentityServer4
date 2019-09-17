using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LBDIdentityServer4.Quickstart.User
{
    public class EditModel
    {
        public string id { get; set; }
        [Remote(nameof(UserController.UserNameExist), "User", ErrorMessage = "用户名已存在")]
        public string user_name { get; set; }

        public string email { get; set; }


        public List<string> claims { get; set; }
       
    }
}
