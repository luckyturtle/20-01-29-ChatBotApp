/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Link.Data;
using Link.Hubs;
using Link.Models;
using Link.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace Link
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IEmailLogs, EmailLogs>();
            services.AddTransient<IUserLog, UserLog>();

            services.AddDetection();
            services.AddDetectionCore().AddBrowser();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });


            services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
            });




            //services.AddAuthentication()
            // .AddMicrosoftAccount(microsoftOptions =>
            // {
            //     microsoftOptions.ClientId = Configuration["Authentication:Microsoft:ApplicationId"];
            //     microsoftOptions.ClientSecret = Configuration["Authentication:Microsoft:Password"];

            // })
            // // .AddGoogle(googleOptions => { ... })
            // .AddTwitter(twitterOptions =>
            // {
            //     twitterOptions.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
            //     twitterOptions.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
            // })
            // .AddFacebook(facebookOptions =>
            // {
            //     facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
            //     facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            // });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\comments-files")),
                RequestPath = ""
            });

            app.UseAuthentication();


            app.UseSignalR(routes =>
            {
                routes.MapHub<NotificationHub>("/notificationHub");
                routes.MapHub<WebRtcHub>("/WebRtcHub");
            });


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "areas",
                  template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
            });

        }
    }
}
