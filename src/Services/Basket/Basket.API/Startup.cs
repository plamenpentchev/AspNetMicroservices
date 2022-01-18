using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Discount.Grpc.Protos;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace Basket.API
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
            //adding the IDistributedCache(Redis) to DI container
            services.AddStackExchangeRedisCache( opts =>
                {
                    opts.Configuration = Configuration.GetValue<string>("CacheSettings:ConnectionString");
                }
            );

            services.AddApiVersioning(config => {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });
            //General configuration, add repository to DI container
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddAutoMapper(typeof(Startup));

            //Grpc configuration, sync communication, add compiler generated grpc client to the DI container, to be used
            //in the DiscountGrpcService abstraction.
            services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>
                ( o => o.Address = new Uri(Configuration.GetValue<string>("GrpcSettings:DiscountUrl")));

            //add the DiscountGrpcService abstraction itself.
            services.AddScoped<DiscountGrpcService>();

            // MassTransit-Rabitmq configuration,async communication.
            services.AddMassTransit( config => 
            {
                config.UsingRabbitMq(configure: 
                    (ctx, cfg) => 
                    {
                        cfg.Host(Configuration.GetValue<string>("EventBusSettings:HostAddress"));
                    });
            });

            services.AddMassTransitHostedService();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Basket.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API v1"));
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
