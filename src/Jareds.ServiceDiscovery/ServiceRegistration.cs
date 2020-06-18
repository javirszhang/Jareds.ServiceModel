using System;
using System.Collections.Generic;
using System.Text;

namespace Jareds.ServiceModel
{
    public class ServiceRegistration
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public uint Port { get; set; }
        public ServiceCheck Check { get; set; }
        public ServiceCenter ServiceCenter { get; set; }
        public List<ServiceProxypass> Proxypass { get; set; }
    }

    public class ServiceCheck
    {
        public TimeSpan DeregisterAfter { get; set; }
        public TimeSpan Interval { get; set; }
        public string Url { get; set; }
        public TimeSpan Timeout { get; set; }
    }
    public class ServiceProxypass
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
    }
    public class ServiceCenter
    {
        public string Host { get; set; }
        public string Name { get; set; }
    }
}
