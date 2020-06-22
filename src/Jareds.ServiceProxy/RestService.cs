using Jareds.ServiceModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;

namespace Jareds.ServiceProxy
{
    public class RestService
    {
        internal static IServiceProvider ServiceProvider;
        /// <summary>
        /// 直接指定host
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <returns></returns>
        public static T For<T>(string host, HttpMessageHandler handler = null)
        {
            WebApiRequest.InitializeHttpClient(handler);
            return DynamicProxy.CreateInstance<T>(host);
        }
        /// <summary>
        /// 使用服务发现获取host
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="discovery"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static T For<T>(IServiceDiscovery discovery, HttpMessageHandler handler = null)
        {
            WebApiRequest.InitializeHttpClient(handler);
            string host = discovery.GetHost();
            return For<T>(host);
        }
        /// <summary>
        /// 使用DI获取服务host
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static T For<T>(HttpMessageHandler handler = null)
        {
            if (ServiceProvider == null)
            {
                throw new InvalidOperationException("missing dependency injection container");
            }
            var discovery = ServiceProvider.GetService(typeof(IServiceDiscovery)) as IServiceDiscovery;
            return For<T>(discovery, handler);
        }
        /// <summary>
        /// 使用指定的HttpClient，HttpClient 需指定 BaseAddress
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <returns></returns>
        public static T For<T>(HttpClient client)
        {
            if (client == null || client.BaseAddress == null)
            {
                throw new InvalidOperationException("HttpClient or HttpClient.BaseAddress cannot be null");
            }
            WebApiRequest.InitializeHttpClient(client);
            string host = client.BaseAddress.Scheme + "://" + client.BaseAddress.Authority;
            return For<T>(host);
        }
    }
}
