using System;
using System.Collections.Generic;
using System.Text;

namespace Jareds.ServiceModel
{
    public class PostAttribute : HttpProxyAttribute
    {
        public PostAttribute(string path) : base("POST", path) { }
    }
}
