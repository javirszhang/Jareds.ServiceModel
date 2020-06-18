using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Jareds.ServiceProxy
{
    internal class WebApiRequest
    {
        public T SendRequest<T>(string url, string httpMethod, object data)
        {
            var dict = data as IDictionary<string, object>;
            var apiUrl = GetApiUrl(url, dict);
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
            return JsonConvert.DeserializeObject<T>(result);

        }

        private async Task<string> SendRequestAsync(HttpRequestMessage message)
        {
            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(message);
                return await response.Content.ReadAsStringAsync();
            }
        }
        private static string GetApiUrl(string apiUrl, IDictionary<string, object> data)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();
            if (data != null)
            {
                data.Aggregate(query, (q, kv) =>
                {
                    if (kv.Value != null)
                    {
                        var t = kv.Value.GetType();
                        if (!t.IsClass || t == typeof(string))
                        {
                            q.Add(kv.Key, kv.Value.ToString());
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
    }
}
