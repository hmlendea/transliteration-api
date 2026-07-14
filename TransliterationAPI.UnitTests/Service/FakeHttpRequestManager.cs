using System.Collections.Generic;
using System.Threading.Tasks;

using TransliterationAPI.Service;

namespace TransliterationAPI.UnitTests.Service
{
    public sealed class FakeHttpRequestManager : IHttpRequestManager
    {
        private string postResponseToReturn = string.Empty;
        private string cookiesToReturn = string.Empty;

        public void SetPostResponseToReturn(string response)
        {
            postResponseToReturn = response;
        }

        public void SetCookiesToReturn(string cookies)
        {
            cookiesToReturn = cookies;
        }

        public Task<string> Post(string url, IDictionary<string, string> formData)
            => Task.FromResult(postResponseToReturn);

        public Task<string> Post(string url, IDictionary<string, string> formData, IDictionary<string, string> headers)
            => Task.FromResult(postResponseToReturn);

        public Task<string> RetrieveCookies(string url)
            => Task.FromResult(cookiesToReturn);
    }
}
