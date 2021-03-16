using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NTccTransaction.Aop;

namespace NTccTransaction.Http.Capital.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// Autofac 容器
        /// </summary>
        public ILifetimeScope AutofacContainer { get; private set; }

        /// <summary>
        /// 依赖注入服务集合
        /// </summary>
        public IServiceCollection Services { get; private set; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NTccTransaction.Http.Capital.WebApi", Version = "v1" });
            });

            services.AddTransient<ICapitalService, CapitalService>();
            services.AddTransient<CapitalService>();

            services.AddNTccTransaction((option) =>
            {
                option.UseSqlServer((oracleOption) =>
                {
                    oracleOption.ConnectionString = Configuration.GetConnectionString("capitalDb");
                });

                option.UseCastleInterceptor(); // inject castle interceptor
            });

            Services = services;
        }

        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(Services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net();

            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NTccTransaction.Http.Capital.WebApi v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
