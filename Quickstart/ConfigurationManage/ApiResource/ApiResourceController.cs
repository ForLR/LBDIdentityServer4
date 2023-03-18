using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using LBDIdentityServer4.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LBDIdentityServer4.Quickstart.ConfigurationManage.ApiResource
{
    public class ApiResourceController : Controller
    {
        private readonly ConfigurationDbContext _context;
        public ApiResourceController(ConfigurationDbContext context)
        {
            this._context = context;
        }
        public async Task<IActionResult> Index()
        {
            var data =  await _context.ApiResources.ToListAsync();
            var result = new List<ApiResourceViewModel>();
            data?.ForEach(x =>
            {
                result.Add(new ApiResourceViewModel
                {
                    name = x.Name,
                    create_time = x.Created,
                    enabled = x.Enabled,
                    scope = new List<string>()
                    //scope=new List<string> {x.Scopes. }
                });
            });
            return View(result);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = new AddApiResourceModel()
            {
                claims = new List<string>
                {
                      Constants.CreateOperationName,
                      Constants.DeleteOperationName,
                      Constants.ReadOperationName,
                      Constants.UpdateOperationName,
                }
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddApiResourceModel args)
        {

            var claims = new List<IdentityServer4.EntityFramework.Entities.ApiResourceClaim>();
            args.claims?.ForEach(x =>
            {
                claims.Add(new IdentityServer4.EntityFramework.Entities.ApiResourceClaim
                {
                    Type = x
                });
            });

            _context.ApiResources.Add(new IdentityServer4.EntityFramework.Entities.ApiResource
            {
                Name = args.name,
                UserClaims = claims,
                Scopes = new List<IdentityServer4.EntityFramework.Entities.ApiResourceScope> {
                   
                    new IdentityServer4.EntityFramework.Entities.ApiResourceScope
                    {
                        Scope=args.name,
                    } }

            });
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return RedirectToAction("Index");
            }
            return View(args);
        }
    }
}