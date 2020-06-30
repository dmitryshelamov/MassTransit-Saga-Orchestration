using System;
using System.Reflection;
using Booking.SagaOrchestrator.Storage;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.Saga;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Abstractions;

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

        public static IServiceCollection AddEntityFrameworkSaga(this IServiceCollection services,
            IConfiguration configuration)
        {
            var config = configuration.GetSection("MassTransitHostConfiguration")
                .Get<MassTransitHostConfiguration>();
            services.AddSingleton(config);
            services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<BookingStateMachine, BookingState>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Pessimistic; // or use Optimistic, which requires RowVersion

                        r.AddDbContext<DbContext, BookingDbContext>((provider, builder) =>
                        {
                            builder.UseSqlServer(configuration.GetConnectionString("DbContext"), m =>
                            {
                                m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                                m.MigrationsHistoryTable($"__{nameof(BookingDbContext)}");
                            });
                        });
                    });

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
                        e =>
                        {
                            var sp = services.BuildServiceProvider();
                            // e.StateMachineSaga(context.Container.GetRequiredService<BookingStateMachine>(),
                            // context.Container.GetRequiredService<ISagaRepository<BookingState>>());
                            e.StateMachineSaga(sp.GetRequiredService<BookingStateMachine>(), sp.GetRequiredService<ISagaRepository<BookingState>>());
                        });
                }));
                services.AddMassTransitHostedService();
            });


            return services;
        }
    }
}