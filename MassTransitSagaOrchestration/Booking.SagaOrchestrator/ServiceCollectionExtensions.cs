using System;
using System.Reflection;
using MassTransit;
using MassTransit.Saga;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.SagaOrchestrator
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInMemorySaga(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("MassTransitHostConfiguration")
                .Get<MassTransitHostConfiguration>();
            services.AddSingleton(config);
            services.AddMassTransit(x =>
            {
                var saga = new BookingStateMachine();
                var repo = new InMemorySagaRepository<BookingState>();


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
                        e => { e.StateMachineSaga(saga, repo); });
                }));
                services.AddMassTransitHostedService();
            });

            return services;
        }
    }
}