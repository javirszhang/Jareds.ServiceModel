using Jareds.ServiceModel;
using System;

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
        public static T For<T>(string host)
        {
            return DynamicProxy.CreateInstance<T>(host);
        }
        /// <summary>
        /// 使用服务发现获取host
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="discovery"></param>
        /// <returns></returns>
        public static T For<T>(IServiceDiscovery discovery)
        {
            string host = discovery.GetHost();
            return For<T>(host);
        }
        /// <summary>
        /// 使用DI获取服务host
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T For<T>()
        {
            if (ServiceProvider == null)
            {
                throw new InvalidOperationException("missing dependency injection container");
            }
            var discovery = ServiceProvider.GetService(typeof(IServiceDiscovery)) as IServiceDiscovery;
            return For<T>(discovery);
        }
    }
}
