using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cons= IdentityModel.OidcConstants;

namespace LBDIdentityServer4.Quickstart.ConfigurationManage
{
    [AllowAnonymous()]
    public class ConfigurationManageController : Controller
    {
        
        private readonly ConfigurationDbContext _context;
        public ConfigurationManageController(ConfigurationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {

            var d = _context.Clients.ToList();
            return View();
        }
        #region Client
        [HttpGet]
        public async Task<IActionResult> Clients()
        {
            var clients = await _context.Clients.Select(x => new ClientsViewModel {
                client_id = x.ClientId,
                redirect_uris = x.RedirectUris.Select(t=>t.RedirectUri).FirstOrDefault(),
                allowed_cors_origins = x.AllowedCorsOrigins.Select(r => r.Origin).ToList(),
                allowed_grant_types = x.AllowedGrantTypes.Select(r => r.GrantType).ToList(),
                allowed_scopes = x.AllowedScopes.Select(r => r.Scope).ToList(),
                client_name = x.ClientName,
                post_logout_redirect_uris = x.PostLogoutRedirectUris.Select(r => r.PostLogoutRedirectUri).FirstOrDefault(),
                client_secrets = x.ClientSecrets.Select(t => t.Value).FirstOrDefault()
            }).ToListAsync();

            return View(clients);
        }

        [HttpGet]
        public IActionResult AddClients()
        {
            var result = new ClientsViewModel
            {
                allowed_grant_types=new List<string>

                {
                    cons.GrantTypes.AuthorizationCode,
                    cons.GrantTypes.ClientCredentials,
                    cons.GrantTypes.DeviceCode,
                     cons.GrantTypes.Implicit,
                     cons.GrantTypes.JwtBearer,
                     cons.GrantTypes.Password,
                     cons.GrantTypes.RefreshToken,
                     cons.GrantTypes.Saml2Bearer,
                     cons.GrantTypes.TokenExchange
                }
            };

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddClients(ClientsViewModel args)
        {
            if (ModelState.IsValid)
            {
                var scoper = new List<ClientScope>();
                args.allowed_scopes?.ForEach(x=> { scoper.Add(new ClientScope { Scope=x}); });
                var corss = new List<ClientCorsOrigin>();
                args.allowed_cors_origins?.ForEach(x => { corss.Add(new ClientCorsOrigin { Origin=x}); });
                var grants=new  List<ClientGrantType>();
                args.allowed_grant_types?.ForEach(x => { grants.Add(new ClientGrantType { GrantType = x }); });
                var client = new IdentityServer4.EntityFramework.Entities.Client
                {
                    ClientId = args.client_id,
                    ClientName = args.client_name,
                    ClientSecrets = new List<ClientSecret> { new ClientSecret { Value = args.client_secrets.Sha256() } },
                    AllowedScopes = scoper,
                    AllowedCorsOrigins = corss,
                   // RedirectUris = new List<ClientRedirectUri> { new ClientRedirectUri { RedirectUri = args.redirect_uris } },
                    //PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri> { new ClientPostLogoutRedirectUri { PostLogoutRedirectUri=args.post_logout_redirect_uris} },
                    AllowedGrantTypes= grants
                };
                _context.Clients.Add(client);
                var resutlt= await _context.SaveChangesAsync();
                if (resutlt>0)
                {
                    return RedirectToAction("Clients");
                }
                ModelState.AddModelError(string.Empty,"Client添加失败");
                return View(args);
            }
           
            ModelState.AddModelError(string.Empty, "参数验证错误");
            return RedirectToAction("AddClients");
        }

        #endregion
    }
}