﻿using Autofac;
using Microsoft.Extensions.Configuration;
using Nancy.Bootstrapper;
using NLog;
using RawRabbit;
using RawRabbit.vNext;
using Warden.Common.Commands;
using Warden.Common.Commands.ApiKeys;
using Warden.Common.Commands.WardenChecks;
using Warden.Common.Commands.Wardens;
using Warden.Common.Events;
using Warden.Common.Events.ApiKeys;
using Warden.Common.Events.Users;
using Warden.Common.Events.Wardens;
using Warden.Services.Extensions;
using Warden.Services.Features.Handlers;
using Warden.Services.Features.Repositories;
using Warden.Services.Features.Services;
using Warden.Services.Features.Settings;
using Warden.Services.Mongo;
using Warden.Services.Nancy;

namespace Warden.Services.Features.Framework
{
    public class Bootstrapper : AutofacNancyBootstrapper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration _configuration;
        public static ILifetimeScope LifetimeScope { get; private set; }

        public Bootstrapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope container)
        {
            base.ConfigureApplicationContainer(container);
            container.Update(builder =>
            {
                builder.RegisterInstance(_configuration.GetSettings<MongoDbSettings>());
                builder.RegisterInstance(_configuration.GetSettings<FeatureSettings>());
                builder.RegisterInstance(_configuration.GetSettings<PaymentPlanSettings>());
                builder.RegisterModule<MongoDbModule>();
                builder.RegisterType<MongoDbInitializer>().As<IDatabaseInitializer>();
                builder.RegisterType<DatabaseSeeder>().As<IDatabaseSeeder>();
                builder.RegisterInstance(BusClientFactory.CreateDefault()).As<IBusClient>();
                builder.RegisterType<UserRepository>().As<IUserRepository>();
                builder.RegisterType<PaymentPlanRepository>().As<IPaymentPlanRepository>();
                builder.RegisterType<UserPaymentPlanRepository>().As<IUserPaymentPlanRepository>();
                builder.RegisterType<UserFeaturesManager>().As<IUserFeaturesManager>();
                builder.RegisterType<UserPaymentPlanService>().As<IUserPaymentPlanService>();
                builder.RegisterType<UserPaymentPlanService>().As<IUserPaymentPlanService>();
                builder.RegisterType<RequestNewApiKeyHandler>().As<ICommandHandler<RequestNewApiKey>>();
                builder.RegisterType<ApiKeyCreatedHandler>().As<IEventHandler<ApiKeyCreated>>();
                builder.RegisterType<RequestProcessWardenCheckResultHandler>()
                    .As<ICommandHandler<RequestProcessWardenCheckResult>>();
                builder.RegisterType<WardenCheckResultProcessedHandler>()
                    .As<IEventHandler<WardenCheckResultProcessed>>();
                builder.RegisterType<UserCreatedHandler>().As<IEventHandler<UserCreated>>();
                builder.RegisterType<RequestCreateWardenHandler>().As<ICommandHandler<RequestCreateWarden>>();
                builder.RegisterType<WardenCreatedHandler>().As<IEventHandler<WardenCreated>>();
            });
            LifetimeScope = container;
        }

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            var databaseSettings = container.Resolve<MongoDbSettings>();
            var databaseInitializer = container.Resolve<IDatabaseInitializer>();
            databaseInitializer.InitializeAsync();
            if (databaseSettings.Seed)
            {
                var seeder = container.Resolve<IDatabaseSeeder>();
                seeder.SeedAsync();
            }
            pipelines.AfterRequest += (ctx) =>
            {
                ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                ctx.Response.Headers.Add("Access-Control-Allow-Headers", "Authorization, Origin, X-Requested-With, Content-Type, Accept");
            };
            Logger.Info("Warden.Services.Features API Started");
        }
    }
}