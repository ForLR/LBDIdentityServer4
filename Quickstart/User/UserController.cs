using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LBDIdentityServer4.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LBDIdentityServer4.Quickstart.User
{
    //[Authorize(Roles ="admin")]
    [Authorize(Policy = "MyPolicy")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManage;
        private readonly IMemoryCache _memoryCache;
        public UserController(UserManager<ApplicationUser> userManage, IMemoryCache memoryCache)
        {
            this._userManage = userManage;
            this._memoryCache = memoryCache;
        }

        
        public async Task<IActionResult> Index()
        {
            var result = await _userManage.Users.Where(x=>x.IsDelete==false).ToListAsync();
            return View(result);
        }




        #region  register

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel args)
        {
            if (!ModelState.IsValid)
            {
                return View(args);
            }

            var user = new ApplicationUser
            {
                UserName = args.UserName,
                Email = args.Email,
                
            };
            var result = await _userManage.CreateAsync(user, args.PassWord);
            if (result.Succeeded)
            {
                return Redirect("account/Index");
            }
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }
            return View(args);
        }

        #endregion

        #region Edit
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManage.FindByIdAsync(id);
            if (user == null)
            {
                return Redirect("Index");
            }
            var claims = await _userManage.GetClaimsAsync(user);
            var mode = new EditModel
            {
                email = user.Email,
                id = user.Id,
                user_name = user.UserName,
                claims = claims.Select(x => x.Value).ToList()
            };
            return View(mode);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditModel args)
        {
            var user = await _userManage.FindByIdAsync(args.id);
            if (user==null)
            {
                return Redirect("Edit");
            }
            user.UserName = args.user_name;
            user.Email = args.email;
            var result = await _userManage.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(string.Empty, "更新条目出错");
            }
            return View(user);
        }



        #endregion

        #region Delete

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManage.FindByIdAsync(id);
            if (user == null || user.NormalizedUserName == User.Identity.Name)
            {
                return View();
            }
            user.IsDelete = true;
            var result = await _userManage.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Redirect("Index");
            }
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(string.Empty, "删除出错");
            }
            return View(user);
        }
        #endregion


        #region Claims

        [HttpGet]
        public async Task<IActionResult> ManageClaims(string id)
        {
            var user = await _userManage.FindByIdAsync(id);
            if(user==null) return Redirect("Index");
            List<string> AllClaimTypeList = new List<string>
            {
                "Edit Albums",
                "Edit Users",
                "Edit Roles",
                "Email"
            };

            var userClaims =await _userManage.GetClaimsAsync(user);
            var claims = AllClaimTypeList.Except(userClaims.Select(x => x.Type)).ToList();
            var vm = new ManageClaimsModel
            {
                UserId = user.Id,
                AvailableClaims = claims
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> ManageClaims(ManageClaimsModel args)
        {
            var user = await _userManage.FindByIdAsync(args.UserId);
            if (user == null)
                RedirectToAction("Index");
            var claim = new Claim(args.ClaimId, args.ClaimId);
            var result =await  _userManage.AddClaimAsync(user,claim);
            if (result.Succeeded)
                return RedirectToAction("Edit", new { user.Id });
            ModelState.AddModelError(string.Empty, "编辑用户Claims出错");
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveClaim(string id ,string claim)
        {
            var user = await _userManage.FindByIdAsync(id);
            if (user == null) return Redirect("Index");
         
            var result=await _userManage.RemoveClaimAsync(user,new Claim(claim, claim));
            if (result.Succeeded) return RedirectToAction("Edit", new { id });
            ModelState.AddModelError(string.Empty, "删除用户Claims出错");
            return View("ManageClaims", new { id });
        }
        #endregion



        #region ServerValidata
        [AllowAnonymous()]
        public async Task<IActionResult> UserNameExist([Bind("UserName")] string userName)
        {
            var user = await _userManage.FindByNameAsync(userName);
            if (user != null)
            {
                return Json("角色已经存在");
            }
            else
                return Json(true);
        }


        #endregion
    }
}