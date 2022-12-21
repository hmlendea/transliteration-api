using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TransliterationAPI.Service
{
    public class HttpRequestManager : IHttpRequestManager
    {
        readonly CookieContainer cookies;
        readonly HttpClient client = new HttpClient();

        public HttpRequestManager()
        {
            cookies = new CookieContainer();
            
            HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = cookies
            };

            client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(3)
            };
        }

        public async Task<string> Post(
            string url,
            IDictionary<string, string> formData)
        {
            using (HttpContent requestContent = new FormUrlEncodedContent(formData))
            {
                using (HttpResponseMessage response = await client
                    .PostAsync(url, requestContent)
                    .ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();

                    return await response
                        .Content
                        .ReadAsStringAsync()
                        .ConfigureAwait(false);
                }
            }
        }

        public async Task<string> Post(
            string url,
            IDictionary<string, string> formData,
            IDictionary<string, string> headers)
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(formData)
            };
            
            foreach (KeyValuePair<string, string> header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            using (HttpResponseMessage response = await client
                .SendAsync(request)
                .ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                return await response
                    .Content
                    .ReadAsStringAsync()
                    .ConfigureAwait(false);
            }
        }

        public async Task<string> RetrieveCookies(string url)
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            
            using (HttpResponseMessage response = await client
                .SendAsync(request)
                .ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                string cook = string.Empty;

                foreach (var cookie in cookies.GetCookies(request.RequestUri))
                {
                    cook += cookie.ToString() + ";";
                }

                return cook;
            }
        }
    }
}
