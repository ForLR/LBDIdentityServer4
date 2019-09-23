// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Reflection;
using IdentityServer4.Models;
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
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.Authorization;


namespace LBDIdentityServer4
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }

        public IConfiguration _configuration { get; }

        public Startup(IHostingEnvironment environment, IConfiguration configuration)
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
                  policy.AddRequirements(new OperationAuthorizationRequirement() { Name= "Create" });
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
          
            services.AddAuthentication()
                .AddGoogle(option=> 
                {
                    option.ClientId = "123";
                    option.ClientSecret = "qw";
                });
            services.AddSingleton<IAuthorizationHandler, ContactIsOwnerAuthorizationHandler>();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            // uncomment if you want to support static files
            app.UseStaticFiles();

            app.UseIdentityServer();

            // uncomment, if you want to add an MVC-based UI
            app.UseMvcWithDefaultRoute();
        }
    }
}
