using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LBDIdentityServer4.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LBDIdentityServer4.Quickstart.Role
{
    /// <summary>
    /// 角色
    /// </summary>
    [Authorize]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManage;
        private readonly UserManager<ApplicationUser> _userManager;
     
        public RoleController(RoleManager<IdentityRole> roleManage, UserManager<ApplicationUser> userManager)
        {
            this._roleManage = roleManage;
            this._userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var result =await _roleManage.Roles.ToListAsync();
            return View(result);
        }

        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public async  Task<IActionResult> AddRole(AddRole args)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            var role = new IdentityRole
            {
                Name=args.role_name,
                NormalizedName=args.role_name
            };
           var result = await _roleManage.CreateAsync(role);

           
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, "添加role失败");
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManage.FindByIdAsync(id);
            if (role==null)
            {
                return View("Index");
            }

            var model = new EditRoleModel
            {
                id = id,
                name = role.Name,
                users=new List<string>()
                
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleModel args)
        {
            var role = await _roleManage.FindByIdAsync(args.id);
            if (role!=null)
            {
                role.Name = args.name;
                var result = await _roleManage.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, "编辑role出错");
            }
            ModelState.AddModelError(string.Empty, "未找到role");
            return View("Index");
        }




        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role =await _roleManage.FindByIdAsync(id);
            if (role!=null)
            {
                var result =await  _roleManage.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, "删除role出错");
            }
            ModelState.AddModelError(string.Empty, "未找到role");
            return View("Index");
        }

        [HttpGet]
        public async Task<IActionResult> AddUserToRole(string roleId)
        {
            var role = await _roleManage.FindByIdAsync(roleId);
            if (role==null)return RedirectToAction("Idnex");
            var result = new RoleUserViewModel()
            {
                role_id = role.Id,
            };
            var users =await _userManager.Users.ToListAsync();

            foreach (var item in users)
            {
                if (!await _userManager.IsInRoleAsync(item,role.Name))
                {
                    result.users.Add(item);
                }
            }
            
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToRole(RoleUserViewModel args)
        {
            var role = await _roleManage.FindByIdAsync(args.role_id);
            var user = await _userManager.FindByIdAsync(args.user_id);
            if (role!=null&&user!=null)
            {
                var result = await _userManager.AddToRoleAsync(user,role.Name);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, "添加角色用户出错");

            }
            ModelState.AddModelError(string.Empty, "用户或者角色未找到");
            return View(args);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUserFromRole(string roleId)
        {
            var role = await _roleManage.FindByIdAsync(roleId);
            if (role == null) return RedirectToAction("Idnex");
            var result = new RoleUserViewModel()
            {
                role_id = role.Id,
            };
            var users = await _userManager.Users.ToListAsync();

            foreach (var item in users)
            {
                if (await _userManager.IsInRoleAsync(item, role.Name))
                {
                    result.users.Add(item);
                }
            }

            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUserFromRole(RoleUserViewModel args)
        {
            var role = await _roleManage.FindByIdAsync(args.role_id);
            var user = await _userManager.FindByIdAsync(args.user_id);
            if (role != null && user != null)
            {
                if (await _userManager.IsInRoleAsync(user,role.Name))
                {
                    var result = await _userManager.RemoveFromRoleAsync(user,role.Name);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "移除角色用户出错");
                    return View(args);
                }
            }
            ModelState.AddModelError(string.Empty, "用户或者角色未找到");
            return View(args);

        }


    }
}