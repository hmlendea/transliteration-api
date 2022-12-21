using System.Collections.Generic;
using System.Threading.Tasks;

namespace TransliterationAPI.Service
{
    public interface IHttpRequestManager
    {
        Task<string> Post(string url, IDictionary<string, string> formData);

        Task<string> Post(string url, IDictionary<string, string> formData, IDictionary<string, string> headers);

        Task<string> RetrieveCookies(string url);
    }
}
