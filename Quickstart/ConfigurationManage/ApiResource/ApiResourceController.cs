using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
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
        public IActionResult Index()
        {

          
            return View();
        }

        public async Task<IActionResult> AddApiResource(AddApiResourceModel args)
        {

            _context.ApiResources.Add(new IdentityServer4.EntityFramework.Entities.ApiResource
            {
                UserClaims = new List<IdentityServer4.EntityFramework.Entities.ApiResourceClaim> { new IdentityServer4.EntityFramework.Entities.ApiResourceClaim { } },
                Scopes = new List<IdentityServer4.EntityFramework.Entities.ApiScope> { new IdentityServer4.EntityFramework.Entities.ApiScope { } },
                
            });
            var result = _context.SaveChanges();

            return View();
        }
    }
}