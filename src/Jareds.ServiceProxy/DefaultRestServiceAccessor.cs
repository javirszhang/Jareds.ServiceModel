using Jareds.ServiceModel;
using System;
using System.Collections.Generic;
using System.Text;

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
                throw new InvalidOperationException("dependency container does not contain IServiceDiscovery implemention");
            }
            return discovery.GetHost();
        }
    }
}
