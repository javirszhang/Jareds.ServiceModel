using Jareds.ServiceModel;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Text;
using Winner.Framework.Utils;

namespace Jareds.ServiceProxy
{
    public class DefaultRestServiceAccessor : RestServiceBaseAccessor
    {
        protected override string GetHost()
        {
            IServiceDiscovery discovery = null;
            if (RestService.ServiceProvider != null)
            {
                discovery = RestService.ServiceProvider.GetService(typeof(IServiceDiscovery)) as IServiceDiscovery;
            }
            if (discovery == null)
            {
                //discovery = new DefaultServiceDiscovery();
                throw new InvalidOperationException("dependency container does not contain IServiceDiscovry implemention");
            }
            return discovery.GetHost();
        }
    }
    /*
    public class DefaultServiceDiscovery : IServiceDiscovery
    {
        private readonly GatewaySetting setting;
        public DefaultServiceDiscovery(IOptions<GatewaySetting> options)
        {
            this.setting = options.Value;
        }
        public string GetHost()
        {            ;
            string host = "";
            string url = $"{host}/api/discovery/{name}";
            WebApiRequest request = new WebApiRequest();
            return request.SendRequest<string>(url, "GET", null);
        }
    }
    public class GatewaySetting
    {
        public string Host { get; set; }
    }
    */
}
