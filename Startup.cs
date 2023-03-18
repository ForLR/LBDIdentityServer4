// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using LBDIdentityServer4.Auth;
using LBDIdentityServer4.Data;
using LBDIdentityServer4.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;


namespace LBDIdentityServer4
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public IConfiguration _configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            _configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAuthorization(option =>
            {
                option.AddPolicy("MyPolicy", policy =>
                {
                    policy.RequireRole("admin");
                    policy.AddRequirements(new OperationAuthorizationRequirement() { Name = "Create" });
                });
            });
            // uncomment, if you want to add an MVC-based UI
            string connStr = _configuration.GetConnectionString("DefaultConnection");
            var migationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
          
            services.AddDbContext<ApplicationDbContext>(option=> 
            {
                option.UseMySql(connStr);
            });
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            //配置session的有效时间,单位秒
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(30);
            });
            services.AddMvc();
            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
                
            });
           
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            

            //.AddTestUsers(TestUsers.Users)
            //   .AddInMemoryIdentityResources(Config.GetIdentityResources())
            //   .AddInMemoryApiResources(Config.GetApis())
            //   .AddInMemoryClients(Config.GetClients())
            .AddAspNetIdentity<ApplicationUser>()
            .AddConfigurationStore(option =>
            {

                option.ConfigureDbContext = c => c.UseMySql(connStr, sql => sql.MigrationsAssembly(migationsAssembly));
            })
            .AddOperationalStore(option =>
            {
                option.ConfigureDbContext = c => c.UseMySql(connStr, sql => sql.MigrationsAssembly(migationsAssembly));
                option.EnableTokenCleanup = true;
            })
            ;
            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
               //builder.AddSigningCredential(new SigningCredentials(new System.Security.Cryptography.X509Certificates.X509Certificate2(),""));    
                throw new Exception("need to configure key material");
            }

            services.AddAuthentication();
                //.AddGoogle(option=> 
                //{
                //    option.ClientId = "123";
                //    option.ClientSecret = "qw";
                //});


            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCookiePolicy();
            app.UseSession();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
