using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Jareds.ServiceProxy
{
    internal class WebApiRequest
    {
        private static HttpClient httpClient;
        internal static void InitializeHttpClient(HttpMessageHandler handler)
        {
            if (httpClient == null)
            {
                httpClient = handler == null ? new HttpClient() : new HttpClient(handler);
            }
        }
        internal static void InitializeHttpClient(HttpClient client)
        {
            httpClient = client;
        }
        public T SendRequest<T>(string url, string httpMethod, object data)
        {
            var apiUrl = GetApiUrl(url, httpMethod, data, out IDictionary<string, object> dict);
            var httpMessage = new HttpRequestMessage
            {
                Method = new HttpMethod(httpMethod),
                RequestUri = new Uri(apiUrl),
            };
            if (data != null)
            {
                string json = null;
                if (dict == null)
                {
                    json = JsonConvert.SerializeObject(data);
                }
                else
                {
                    object v = null;
                    foreach (var kv in dict)
                    {
                        if (kv.Value != null && kv.Value.GetType().IsClass && kv.Value.GetType() != typeof(string))
                        {
                            v = kv.Value;
                            break;
                        }
                    }
                    if (v != null)
                    {
                        json = JsonConvert.SerializeObject(v);
                    }
                }
                if (!string.IsNullOrEmpty(json))
                {
                    httpMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }
            }
            var result = SendRequestAsync(httpMessage).Result;
            if (string.IsNullOrEmpty(result))
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(result);

        }
        private async Task<string> SendRequestAsync(HttpRequestMessage message)
        {
            InitializeHttpClient((HttpMessageHandler)null);
            var response = await httpClient.SendAsync(message);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return await Task.FromResult((string)null);
        }
        private static string GetApiUrl(string apiUrl, string method, object data, out IDictionary<string, object> dict)
        {
            bool isGet = "get".Equals(method, StringComparison.OrdinalIgnoreCase);
            dict = data as IDictionary<string, object>;
            if (dict == null && data != null && isGet)
            {
                dict = Object2Dict(data);
            }
            Dictionary<string, string> query = new Dictionary<string, string>();
            if (dict != null)
            {
                dict.Aggregate(query, (q, kv) =>
                {
                    if (kv.Value != null)
                    {
                        var t = kv.Value.GetType();
                        if (isGet || !t.IsClass || t == typeof(string))
                        {
                            string value = t.IsClass && t != typeof(string) ? JsonConvert.SerializeObject(kv.Value) : kv.Value.ToString();
                            q.Add(kv.Key, value);
                        }
                    }
                    return q;
                });
            }
            var matchCollection = Regex.Matches(apiUrl, "/\\{([\\w_]+)([\\?]?)\\}");
            List<string> pathParas = new List<string>();
            foreach (Match match in matchCollection)
            {
                string name = match.Groups[1].Value;
                pathParas.Add(name);
                bool nullable = !string.IsNullOrEmpty(match.Groups[2].Value);
                string value = string.IsNullOrEmpty(query[name]) ? "" : "/" + query[name];
                apiUrl = apiUrl.Replace("/{" + name + (nullable ? "?" : "") + "}", value);
            }
            Dictionary<string, string> queryKeyValues = new Dictionary<string, string>();
            foreach (var item in query)
            {
                if (!pathParas.Contains<string>(item.Key, StringComparer.OrdinalIgnoreCase))
                {
                    queryKeyValues.Add(item.Key, item.Value);
                }
            }
            StringBuilder builder = new StringBuilder();
            queryKeyValues.Aggregate(builder, (b, kv) => b.Append(kv.Key).Append("=").Append(WebUtility.UrlEncode(kv.Value)).Append("&"));
            if (builder.Length > 1)
            {
                builder.Remove(builder.Length - 1, 1);
            }
            string querystring = builder.ToString();
            if (!string.IsNullOrEmpty(querystring))
            {
                apiUrl += "?" + querystring;
            }
            return apiUrl;
        }
        private static IDictionary<string, object> Object2Dict(object data)
        {
            Type t = data.GetType();
            var properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var dict = new Dictionary<string, object>();
            foreach (var p in properties)
            {
                dict.Add(p.Name, p.GetValue(data));
            }
            return dict;
        }
    }
}
