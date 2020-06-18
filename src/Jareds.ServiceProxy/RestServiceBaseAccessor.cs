using System;
using System.Collections.Generic;
using System.Text;

namespace Jareds.ServiceProxy
{
    public abstract class RestServiceBaseAccessor
    {
        public virtual T Handle<T>(string path, string httpMethod, object data, string host)
        {
            if (string.IsNullOrEmpty(host))
            {
                host = GetHost();
            }
            string url = string.Concat(host.TrimEnd('/'), "/", path.TrimStart('/'));
            WebApiRequest apiRequest = new WebApiRequest();
            return apiRequest.SendRequest<T>(url, httpMethod, data);
        }
        protected abstract string GetHost();

        public static RestServiceBaseAccessor Resolver()
        {
            var ins = RestService.ServiceProvider?.GetService(typeof(RestServiceBaseAccessor)) as RestServiceBaseAccessor;
            return ins ?? new DefaultRestServiceAccessor();
        }
    }
}
