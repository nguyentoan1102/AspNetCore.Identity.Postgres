using AspNetCore.Identity.PG.Context;
using AspNetCore.Identity.PG.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using WebExample.Models;
using WebExample.Services;
using IdentityRole = AspNetCore.Identity.PG.IdentityRole;
namespace WebExample
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

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddUserStore<UserStore<ApplicationUser>>()
                .AddRoleStore<RoleStore<IdentityRole>>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                    .AddDefaultTokenProviders();
            // Configure Identity
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(20);
                options.Lockout.MaxFailedAccessAttempts = 10;

                // User settings
                options.User.RequireUniqueEmail = true;
            });
            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            IdentityDbConfig.StringConnectionName = "DefaultCon";

            services.AddAuthentication()

                 .AddCookie(o => o.LoginPath = new PathString("/logout"))

                .AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            }).AddGoogle(googleOptions =>
           {
               googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
               googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
           });
            services.ConfigureApplicationCookie(options => options.ExpireTimeSpan = TimeSpan.FromSeconds(30));
            services.AddMvc();
            //services.AddSingleton(_ => Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
