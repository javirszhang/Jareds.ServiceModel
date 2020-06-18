using System;
using System.Collections.Generic;
using System.Text;

namespace Jareds.ServiceProxy
{
    public class PutAttribute : HttpProxyAttribute
    {
        public PutAttribute(string path) : base("PUT", path) { }
    }
}
