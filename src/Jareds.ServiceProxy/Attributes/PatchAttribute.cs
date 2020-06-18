using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Jareds.ServiceProxy
{
    public class PatchAttribute : HttpProxyAttribute
    {
        public PatchAttribute(string path) : base("PATCH", path) { }
    }
}
