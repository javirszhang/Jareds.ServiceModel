using System;
using System.Collections.Generic;
using System.Text;

namespace Jareds.ServiceModel
{
    /// <summary>
    /// 服务注册
    /// </summary>
    public class ServiceRegistration
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 节点ID
        /// </summary>
        public string NodeId { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 节点uri.host
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 健康检查配置
        /// </summary>
        public ServiceCheck Check { get; set; }
        /// <summary>
        /// 网关服务中心
        /// </summary>
        public ServiceCenter ServiceCenter { get; set; }
        /// <summary>
        /// 代理服务
        /// </summary>
        public List<ServiceProxypass> Proxypass { get; set; }
    }
    /// <summary>
    /// 健康检查配置
    /// </summary>
    public class ServiceCheck
    {
        /// <summary>
        /// 反注册时间
        /// </summary>
        public TimeSpan DeregisterAfter { get; set; }
        /// <summary>
        /// 间隔
        /// </summary>
        public TimeSpan Interval { get; set; }
        /// <summary>
        /// 健康检查url地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 超时时间
        /// </summary>
        public TimeSpan Timeout { get; set; }
    }
    /// <summary>
    /// 代理服务
    /// </summary>
    public class ServiceProxypass
    {
        /// <summary>
        /// 唯一id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }
    /// <summary>
    /// 网关服务中心
    /// </summary>
    public class ServiceCenter
    {
        /// <summary>
        /// 网关主机
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 网关名称
        /// </summary>
        public string Name { get; set; }
    }
}
