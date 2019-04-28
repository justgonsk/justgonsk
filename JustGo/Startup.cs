using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustGo.Data;
using JustGo.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using JustGo.Controllers;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using JustGoModels.Interfaces;
using JustGoUtilities;

namespace JustGo
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver
                        = Utilities.SnakeCaseSettings.ContractResolver;
                })
                .AddControllersAsServices();

            services.AddHttpContextAccessor();

            services.AddCors();
            services.AddHttpClient();

            services.AddDbContext<MainContext>(options =>
            {
                options.UseLazyLoadingProxies(); //это нужно и для in-memory базы тоже

                options.UseInMemoryDatabase("justgo_inmemory");
                /*var databaseConnStr = Environment.IsDevelopment() ? "LocalEventContext" : "TODO!!!";

                var connectionString = Configuration.GetConnectionString(databaseConnStr);

                options.UseSqlServer(connectionString);*/
            });

            services.AddScoped<IEventsRepository, DbEventsRepository>();
            services.AddScoped<IPlacesRepository, DbPlacesRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(builder => builder.AllowAnyOrigin());

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}