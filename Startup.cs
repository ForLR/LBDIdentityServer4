// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Reflection;
using LBDIdentityServer4.Data;
using LBDIdentityServer4.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            var builder = services.AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(option=> 
                {
                    option.ConfigureDbContext = c => c.UseMySql(connStr,sql=>sql.MigrationsAssembly(migationsAssembly));
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
                throw new Exception("need to configure key material");
            }
            services.AddAuthentication()
                .AddGoogle(option=> 
                {
                    option.ClientId = "123";
                    option.ClientSecret = "qw";
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to support static files
            app.UseStaticFiles();

            app.UseIdentityServer();

            // uncomment, if you want to add an MVC-based UI
            app.UseMvcWithDefaultRoute();
        }
    }
}
