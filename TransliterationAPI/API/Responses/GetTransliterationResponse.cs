
using NuciAPI.Responses;

namespace TransliterationAPI.API.Responses
{
    public class GetTransliterationResponse : NuciApiSuccessResponse
    {
        public string Text { get; set; }
    }
}
