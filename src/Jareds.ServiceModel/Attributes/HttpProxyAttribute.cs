using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Jareds.ServiceModel
{
    public class HttpProxyAttribute : Attribute
    {
        public HttpProxyAttribute(string httpMethod, string path)
        {
            this.HttpMethod = httpMethod;
            this.Path = path;
        }
        public string HttpMethod { get; set; }
        public string Path { get; set; }
    }
}
