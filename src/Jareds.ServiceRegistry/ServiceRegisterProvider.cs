using Jareds.ServiceModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Jareds.ServiceRegistry
{
    internal class ServiceRegisterProvider
    {
        public static ServiceRegistration GetServices(ServiceRegistration service)
        {
            List<string> searchedLocations = new List<string>();
            var types = ProbeModules.GetModules<ControllerBase>(searchedLocations);
            service.Proxypass = new List<ServiceProxypass>();
            foreach (var t in types)
            {
                if (t.IsDefined(typeof(NonControllerAttribute)))
                {
                    continue;
                }
                var methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod);
                foreach (var m in methods)
                {

                    if (m.IsDefined(typeof(NonActionAttribute)))
                    {
                        continue;
                    }
                    var hmAttr = m.GetCustomAttribute(typeof(HttpMethodAttribute)) as HttpMethodAttribute;
                    if (hmAttr == null)
                    {
                        continue;
                    }
                    string path = "/";
                    var routeAttr = t.GetCustomAttribute(typeof(RouteAttribute)) as RouteAttribute;
                    if (!string.IsNullOrEmpty(routeAttr?.Template))
                    {
                        path += routeAttr.Template;
                    }
                    if (!string.IsNullOrEmpty(hmAttr.Template))
                    {
                        path += $"/{hmAttr.Template}";
                    }
                    path = Regex.Replace(path, @"^(.+?)(\[.+\])(.*)$", $"$1{t.Name.ToLower().Replace("controller", "")}$3");
                    string httpMethod = string.Join(",", hmAttr.HttpMethods).ToLower();

                    var ppAttr = m.GetCustomAttribute(typeof(ProxypassAttribute)) as ProxypassAttribute;

                    string name = ppAttr?.Name ?? string.Concat(path.Replace("{", "").Replace("}", "").Replace("?", "").Replace("[", "").Replace("]", ""), ".", httpMethod);
                    string id = ppAttr?.ProxyCode ?? name;
                    service.Proxypass.Add(new ServiceProxypass
                    {
                        Path = path,
                        Name = name,
                        Id = id
                    });
                }
            }
            return service;

        }
    }
}
