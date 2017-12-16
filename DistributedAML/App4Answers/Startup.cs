using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using App4Answers.Data;
using App4Answers.Models;
using App4Answers.Services;
using As.A4ACore;
using StructureMap;

namespace App4Answers
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            

            services.AddMvc();
            
            
            services.AddMvc().AddControllersAsServices();

         

            var z = new Registry();

           
            z.Scan(x =>
            {
                x.TheCallingAssembly();
                x.Assembly("As.A4ACore");
                x.WithDefaultConventions();
            });
            
            
            
            var container = new StructureMap.Container();
            container.Configure(config =>
            {
                config.Populate(services);
                config.AddRegistry(z);
                config.ForConcreteType<A4ARepository>().Configure.Ctor<string>("connectionString").Is(Configuration.GetConnectionString("MainLogic"));

                
            });
            
            return container.GetInstance<IServiceProvider>();
            
            
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
