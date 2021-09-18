using Basket.Api.Configs;
using Basket.Api.Controllers.GrpcServices;
using Basket.Api.Controllers.GrpcServices.Interfaces;
using Basket.Api.Repositories;
using Basket.Api.Repositories.Interfaces;
using Discount.Grpc.Protos;
using MassTransit;
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

namespace Basket.Api
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
            AddCacheServices(services);

            AddAppServices(services);

            AddGrpcServics(services);

            AddEventBusServices(services);

            services.AddAutoMapper(typeof(Startup));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Basket.Api", Version = "v1" });
            });
        }

        private void AddAppServices(IServiceCollection services)
        {
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<IDiscountService, DiscountGrpcService>();
        }

        private void AddGrpcServics(IServiceCollection services)
        {
            services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(
                            options => options.Address = new Uri(Configuration["GrpcSettings:DiscountUrl"]));
        }

        private void AddCacheServices(IServiceCollection services)
        {
            services.Configure<RedisConfig>(Configuration.GetSection(RedisConfig.CacheSettings));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration[$"{RedisConfig.CacheSettings}:{nameof(RedisConfig.ConnectionString)}"];
            });
        }

        private void AddEventBusServices(IServiceCollection services)
        {
            services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((context, rabbitConfig) =>
                {
                    rabbitConfig.Host(Configuration["EventBusSettings:HostAddress"]);
                });
            });

            services.AddMassTransitHostedService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.Api v1"));
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