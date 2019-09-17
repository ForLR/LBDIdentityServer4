using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LBDIdentityServer4.Quickstart.User
{
    public class RegisterModel
    {
        [Remote(nameof(UserController.UserNameExist), "User", ErrorMessage = "用户名已存在")]
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string PassWord { get; set; }

    }
}
