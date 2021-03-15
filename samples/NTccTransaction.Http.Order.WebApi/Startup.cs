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
using NTccTransaction.Oracle;
using NTccTransaction.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTccTransaction.Http.Order.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public ILifetimeScope AutofacContainer { get; private set; }

        public IServiceCollection Services { get; private set; }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NTccTransaction.Http.Order.WebApi", Version = "v1" });
            });

            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<OrderService>();

            services.AddTransient<IOrderService1, OrderService1>();
            services.AddTransient<OrderService1>();

            services.AddTransient<ICapitalProxy, CapitalProxy>();
            services.AddTransient<CapitalProxy>();

            services.AddNTccTransaction((option) =>
            {
                option.UseOracle((oracleOption) =>
                {
                    oracleOption.ConnectionString = Configuration.GetConnectionString("orderDb");
                    oracleOption.Version = "11";
                });

                option.UseCastleInterceptor();
            });

            services.AddHttpApi<ICapitalApi>()
                .ConfigureHttpApi(Configuration.GetSection(nameof(ICapitalApi)));

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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NTccTransaction.Http.Order.WebApi v1"));
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
