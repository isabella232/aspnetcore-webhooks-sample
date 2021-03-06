﻿/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using GraphWebhooks_Core.Infrastructure;
using GraphWebhooks_Core.Models;
using GraphWebhooks_Core.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GraphWebhooks_Core
{

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //default system actions and options integration with dependency system
            services.InitializeDefault(Configuration);
            //init AzureAd specific configuration
            services.InitializeAuthentication(Configuration);
            services.AddControllersWithViews();

            services.Configure<KeyVaultOptions>(Configuration.GetSection("KeyVaultSettings"));
            services.Configure<SubscriptionOptions>(Configuration.GetSection("SubscriptionSettings"));
            services.AddSingleton<KeyVaultManager>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(config =>
            {
                config.MapControllers();
                config.MapDefaultControllerRoute();
                config.MapHub<NotificationHub>("/NotificationHub");
            });
        }
    }
}
