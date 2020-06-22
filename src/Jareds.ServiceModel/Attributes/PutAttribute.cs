using System;
using System.Collections.Generic;
using System.Text;

namespace Jareds.ServiceModel
{
    public class PutAttribute : HttpProxyAttribute
    {
        public PutAttribute(string path) : base("PUT", path) { }
    }
}
