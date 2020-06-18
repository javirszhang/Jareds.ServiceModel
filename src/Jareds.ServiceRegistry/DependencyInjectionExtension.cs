using Jareds.ServiceModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jareds.ServiceRegistry
{
    public static class DependencyInjectionExtension
    {
        public static void AddServiceRegistry(IServiceCollection services, ServiceRegistration registration, IServiceCenter client)
        {
            var reg = ServiceRegisterProvider.GetServices(registration);
            services.AddSingleton<IServiceCenter>(sp => client);
            services.AddSingleton<ServiceRegistration>(sp => registration);
            services.AddHostedService<RegisterHostedService>();
        }
        public static void AddServiceRegistry(IServiceCollection services, ServiceRegistration registration)
        {
            AddServiceRegistry(services, registration, new ApiGatewayClient());
        }
        public static void AddServiceRegistry(IServiceCollection services, IConfiguration configuration)
        {
            var registration = new ServiceRegistration();
            configuration.GetSection("serviceRegistry").Bind(registration);
            services.Configure<ServiceRegistration>(configuration.GetSection("serviceRegistry"));
            AddServiceRegistry(services, registration);
        }
        public static void UseHealthCheck(this IApplicationBuilder app)
        {
            app.Map("/health", a =>
            {
                a.Run(r => r.Response.WriteAsync("ok"));
            });
        }
    }
}
