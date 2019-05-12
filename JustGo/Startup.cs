using System;
using System.Net;
using JustGoModels;
using JustGoModels.Interfaces;
using JustGoModels.Models.Auth;
using JustGoUtilities.Data;
using JustGoUtilities.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;

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

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
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

            //Debugger.Launch();
            services.AddDbContext<MainContext>(options =>
            {
                options.UseLazyLoadingProxies(); //это нужно и для in-memory базы тоже

                var connectionString = GetConnectionStringDependingOnEnvironment();

                options.UseMySQL(connectionString,
                    builder => builder.MigrationsAssembly(nameof(JustGo)));
            });

            services.AddDefaultIdentity<JustGoUser>(options =>
                {
                    options.Password.RequiredLength = 5;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<MainContext>()
                .AddDefaultTokenProviders();

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
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(builder => builder.AllowAnyOrigin());

            app.UseHttpsRedirection();

            loggerFactory.AddNLog();

            app.AddNLogWeb();

            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc();
        }

        private string GetConnectionStringDependingOnEnvironment()
        {
            //для локального тестирования
            if (Environment.IsDevelopment())
            {
                return Configuration.GetConnectionString("LocalMySQL");
            }

            //для удалённого тестирования на Heroku

            if (Environment.IsStaging())
            {
                return Configuration.GetConnectionString("HerokuMySQL");
            }

            //для продакшена на яндекс облаке

            if (Environment.IsProduction())
            {
                return Configuration.GetConnectionString("YandexMySQL");
            }

            var message = $"Custom environments, such as {Environment.EnvironmentName}, are not supported";

            throw new NotSupportedException(message);
        }
    }
}