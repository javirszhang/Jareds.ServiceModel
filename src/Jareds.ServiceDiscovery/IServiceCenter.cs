using System;
using System.Collections.Generic;
using System.Text;

namespace Jareds.ServiceModel
{
    public interface IServiceCenter
    {
        void OnStart(ServiceRegistration registration);
        void OnStop(ServiceRegistration registration);
    }
}
