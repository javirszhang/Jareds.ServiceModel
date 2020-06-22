using Microsoft.AspNetCore.Builder;

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
