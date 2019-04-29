using JustGo.Controllers;
using JustGo.Data;
using JustGo.Repositories;
using JustGoModels.Interfaces;
using JustGoUtilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.EntityFrameworkCore.Extensions;
using NLog.Extensions.Logging;
using NLog.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System;

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

                //для локального тестирования (Паша)
                if (Environment.IsDevelopment())
                {
                    options.UseInMemoryDatabase("LocalInMemory");
                }

                //для локального тестирования (Андрей)
                else if (Environment.IsEnvironment("localmssql"))
                {
                    var connectionString = Configuration.GetConnectionString("LocalSQLServer");

                    options.UseSqlServer(connectionString);
                }

                //для удалённого тестирования на Heroku
                else if (Environment.IsStaging())
                {
                    var connectionString = Configuration.GetConnectionString("HerokuPostgres");

                    options.UseNpgsql(connectionString);
                }

                //для продакшена на яндекс облаке
                else if (Environment.IsProduction())
                {
                    var connectionString = Configuration.GetConnectionString("YandexMySQL");

                    options.UseMySQL(connectionString);
                }
            });

            services.AddScoped<IEventsRepository, DbEventsRepository>();
            services.AddScoped<IPlacesRepository, DbPlacesRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
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

            loggerFactory.AddNLog();

            app.AddNLogWeb();

            app.UseMvc();
        }
    }
}