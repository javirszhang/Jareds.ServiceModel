using System;
using System.Collections.Generic;
using System.Text;

namespace Jareds.ServiceModel
{
    public class DeleteAttribute : HttpProxyAttribute
    {
        public DeleteAttribute(string path) : base("DELETE", path)
        {

        }
    }
}
