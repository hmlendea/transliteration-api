using System.Collections.Generic;
using System.Threading.Tasks;

namespace TransliterationAPI.Service
{
    public interface IHttpRequestManager
    {
        Task<string> Post(string url, IDictionary<string, string> formData);
    }
}
