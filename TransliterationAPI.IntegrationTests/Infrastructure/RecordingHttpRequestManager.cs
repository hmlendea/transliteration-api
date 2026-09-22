using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TransliterationAPI.Service;

namespace TransliterationAPI.IntegrationTests.Infrastructure
{
    internal sealed class RecordingHttpRequestManager : IHttpRequestManager
    {
        internal int PostInvocationCount { get; private set; }
        internal int PostWithHeadersInvocationCount { get; private set; }
        internal int RetrieveCookiesInvocationCount { get; private set; }
        internal int TotalPostInvocationCount => PostInvocationCount + PostWithHeadersInvocationCount;
        internal string CookiesToReturn { get; set; } = "translit=Test1234!;other=value";
        internal Exception? ExceptionToThrow { get; set; }
        internal string? LastCookieRetrievalUrl { get; private set; }
        internal IDictionary<string, string>? LastFormData { get; private set; }
        internal IDictionary<string, string>? LastHeaders { get; private set; }
        internal string? LastPostUrl { get; private set; }
        internal string ResponseToReturn { get; set; } = "ack:::Solaire of Astora";

        public Task<string> Post(string url, IDictionary<string, string> formData)
        {
            PostInvocationCount += 1;
            LastPostUrl = url;
            LastFormData = new Dictionary<string, string>(formData);
            LastHeaders = null;

            return CreateResponseTask(ResponseToReturn);
        }

        public Task<string> Post(
            string url,
            IDictionary<string, string> formData,
            IDictionary<string, string> headers)
        {
            PostWithHeadersInvocationCount += 1;
            LastPostUrl = url;
            LastFormData = new Dictionary<string, string>(formData);
            LastHeaders = new Dictionary<string, string>(headers);

            return CreateResponseTask(ResponseToReturn);
        }

        public Task<string> RetrieveCookies(string url)
        {
            RetrieveCookiesInvocationCount += 1;
            LastCookieRetrievalUrl = url;

            return CreateResponseTask(CookiesToReturn);
        }

        private Task<string> CreateResponseTask(string response)
        {
            if (ExceptionToThrow is not null)
            {
                return Task.FromException<string>(ExceptionToThrow);
            }

            return Task.FromResult(response);
        }
    }
}