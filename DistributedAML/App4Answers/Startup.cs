using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using App4Answers.Data;
using App4Answers.Extensions;
using App4Answers.Models;
using App4Answers.Services;
using As.A4ACore;
using As.Logger;
using As.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using StructureMap;
using App4Answers.Models.A4Amodels;

namespace App4Answers
{
    public class A4ALogger : IExternalLogger
    {
        private ILogger<A4ALogger> _log;
        public A4ALogger(ILogger<A4ALogger> log)
        {
            this._log = log;
        }
        public void Log(AsLogEntry entry)
        {
            _log.LogDebug($"{entry.Type},{entry.Message},{entry.Module}");
        }
    }


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
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection_"+Helper.GetPlatform().ToString()))); 

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();            
            
            services.AddMvc().AddControllersAsServices();
            
            services.AddMvc()
                .AddSessionStateTempDataProvider();

            services.AddSession();

            var z = new Registry();
           
            z.Scan(x =>
            {
                x.TheCallingAssembly();
                x.Assembly("As.A4ACore");
                x.Assembly("As.Email");
                x.SingleImplementationsOfInterface();
                x.WithDefaultConventions();
            });

            var container = new StructureMap.Container();

            services.AddSingleton<Container>(container);

            container.Configure(config =>
            {
                config.Populate(services);
                config.AddRegistry(z);
                config.ForConcreteType<A4ARepository>().Configure.Ctor<string>("connectionString").Is(Configuration.GetConnectionString("MainLogic_" + Helper.GetPlatform().ToString()));

                
            });

            var loggerAdapter = container.GetInstance<A4ALogger>();

            L.SetExternalLogger(loggerAdapter,AsLogInfo.Info);



            
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

            app.UseGraphiQl();

            app.UseStatusCodePages();

            app.UseTypeContainer();

            app.UseSession();

            container = app.ApplicationServices.GetService<Container>();

            var z2 = container.GetInstance<InitializeA4ADatabase>();

            if (!z2.IsInitialized())
                z2.Initialize();

            var myModel = container.GetInstance<A4AModel1>();

            var emailService = myModel.GetEmailDefinition();

            if (emailService != null)
            {

                period  = new TimeSpan(0, 0, 0, 0, (int) emailService.DelayMilliseconds);
                timer = new Timer(myCallback, null, period, disablePolling);

            }
            else
            {
                L.Trace("Email service is not configured in database ... continuing but emails will not be polled - check static set up in DB");
            }



            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private Timer timer = null;
        private Container container = null;
        private TimeSpan  period;
        private TimeSpan disablePolling = new TimeSpan(0,0,0,0,-1);

        private Dictionary<string,DateTime> processedEvents = new Dictionary<string, DateTime>();

        void myCallback(Object o)
        {
            var myModel = container.GetInstance<A4AModel1>();
            myModel.PollEmailState(processedEvents);
            timer = new Timer(myCallback,null,period,disablePolling);

        }
    }
}
