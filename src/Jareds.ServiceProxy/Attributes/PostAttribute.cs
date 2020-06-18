using System;
using System.Collections.Generic;
using System.Text;

namespace Jareds.ServiceProxy
{
    public class PostAttribute : HttpProxyAttribute
    {
        public PostAttribute(string path) : base("POST", path) { }
    }
}
