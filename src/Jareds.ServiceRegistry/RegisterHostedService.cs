using Jareds.ServiceModel;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jareds.ServiceRegistry
{
    public class RegisterHostedService : IHostedService
    {
        private readonly IServiceCenter client;
        private readonly ServiceRegistration registration;
        public RegisterHostedService(IServiceCenter client, ServiceRegistration registration)
        {
            this.client = client;
            this.registration = registration;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() => this.client.OnStart(this.registration));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() => this.client.OnStop(this.registration));
        }
    }
}
