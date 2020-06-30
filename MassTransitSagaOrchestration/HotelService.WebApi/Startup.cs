using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Booking.SagaOrchestrator;
using HotelService.WebApi.Consumers;
using HotelService.WebApi.DAL;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace HotelService.WebApi
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
            services.AddDbContextPool<HotelDbContext>(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString("DbContext")));

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelService.WebApi", Version = "1" });
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "HotelService.WebApi.xml");
                c.IncludeXmlComments(filePath);
            });

            //  setup MassTransit
            var config = Configuration.GetSection("MassTransitHostConfiguration")
                .Get<MassTransitHostConfiguration>();
            services.AddSingleton(config);
            services.AddMassTransit(x =>
            {

                x.AddConsumer<BookingConsumer>();

                var baseUri = new Uri(config.RabbitMQAddress);
                var hostUri = new Uri(baseUri, config.HostName);

                x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(hostUri, hostCfg =>
                    {
                        hostCfg.Username(config.UserName);
                        hostCfg.Password(config.Password);
                    });

                    cfg.ReceiveEndpoint(Assembly.GetExecutingAssembly().GetName().Name,
                        e => { e.ConfigureConsumer<BookingConsumer>(context); });

                }));
                services.AddMassTransitHostedService();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelService.WebApi");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
