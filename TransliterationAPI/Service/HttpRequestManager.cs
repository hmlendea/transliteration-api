using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TransliterationAPI.Service
{
    public class HttpRequestManager : IHttpRequestManager
    {
        private HttpClient client = new HttpClient();

        public HttpRequestManager()
        {
            this.client = new HttpClient();
        }

        public async Task<string> Post(string url, IDictionary<string, string> formData)
        {
            using (HttpContent formContent = new FormUrlEncodedContent(formData))
            {
                using (HttpResponseMessage response = await client
                    .PostAsync(url, formContent)
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
    }
}
