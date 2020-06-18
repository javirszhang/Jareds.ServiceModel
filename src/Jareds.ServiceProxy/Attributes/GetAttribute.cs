using System;
using System.Collections.Generic;
using System.Text;

namespace Jareds.ServiceProxy
{
    public class GetAttribute : HttpProxyAttribute
    {
        public GetAttribute(string path) : base("GET", path)
        {

        }
    }
}
