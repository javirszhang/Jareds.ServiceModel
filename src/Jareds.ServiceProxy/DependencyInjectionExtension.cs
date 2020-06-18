using Jareds.ServiceModel;
using Microsoft.AspNetCore.Builder;
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
        public static void UseRestService(this IApplicationBuilder app)
        {
            RestService.ServiceProvider = app.ApplicationServices;
        }
    }
}
