using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jareds.ServiceProxy
{
    public static class DependencyInjectionExtension
    {
        public static void AddRestService<T>(this IServiceCollection services) where T : class, IServiceDiscovery
        {
            services.AddSingleton<IServiceDiscovery, T>();
            RestService.ServiceProvider = services.BuildServiceProvider();
        }
        public static void AddRestService(this IServiceProvider isp)
        {
            RestService.ServiceProvider = isp;
        }
    }
}
