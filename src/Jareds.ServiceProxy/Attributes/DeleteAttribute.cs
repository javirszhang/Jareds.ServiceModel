using System;
using System.Collections.Generic;
using System.Text;

namespace Jareds.ServiceProxy
{
    public class DeleteAttribute : HttpProxyAttribute
    {
        public DeleteAttribute(string path) : base("DELETE", path)
        {

        }
    }
}
