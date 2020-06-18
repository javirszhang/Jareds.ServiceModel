using System;
using System.Collections.Generic;
using System.Text;

namespace Jareds.ServiceProxy
{
    public interface IServiceDiscovery
    {
        string GetHost();
    }
}
