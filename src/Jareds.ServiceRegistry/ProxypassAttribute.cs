using System;
using System.Collections.Generic;
using System.Text;

namespace Jareds.ServiceRegistry
{
    public class ProxypassAttribute : Attribute
    {
        public bool Ignore { get; set; }
        public string ProxyCode { get; set; }
        public string Name { get; set; }
    }
}
