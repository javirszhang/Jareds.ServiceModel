using Jareds.ServiceModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Jareds.ServiceRegistry
{
    public class ApiGatewayClient : IServiceCenter
    {
        public void OnStart(ServiceRegistration registration)
        {
            var json = JsonConvert.SerializeObject(registration);
            SendRequest(registration.ServiceCenter.Host, "api/registry", json);
        }

        public void OnStop(ServiceRegistration registration)
        {
            SendRequest(registration.ServiceCenter.Host, $"api/registry/{registration.Name}/unavailable", null);
        }
        private void SendRequest(string host, string path, string json)
        {
            var uri = new Uri(string.Concat(host.TrimEnd('/'), "/", path.TrimStart('/')));
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage message = new HttpRequestMessage();
                message.Method = HttpMethod.Get;
                message.RequestUri = uri;
                if (!string.IsNullOrEmpty(json))
                {
                    message.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    message.Method = HttpMethod.Post;
                }
                client.SendAsync(message);
            };
        }
    }
}
