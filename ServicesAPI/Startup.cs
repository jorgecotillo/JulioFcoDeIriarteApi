using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServicesAPI.Models;
using ServicesAPI.Interfaces;

namespace ServicesAPI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowAnyHeader());
            });

            services.AddMvcCore()
                    .AddAuthorization()
                    .AddJsonFormatters();

            // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
            // NOTE: Important line to be added otherwise Versioning won't work
            services.AddApiVersioning(o => o.ReportApiVersions = true);

            services.AddSingleton(Configuration);

            services.AddTransient<INotificationStore, NotificationStore>();
            services.AddTransient<INotificationHubStore, NotificationHubStoreModel>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors("AllowAll");

            var authority = Configuration["General:Authority"];

            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = authority,

                ApiName = "api1",
                AllowedScopes = { "api1" },

                RequireHttpsMetadata = false
            });

            app.UseMvc();
        }
    }
}
