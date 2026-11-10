using NuciAPI.Responses;

namespace TransliterationAPI.API.Responses
{
    public sealed class TransliterationResponseContent : NuciApiResponseContent
    {
        public string Text { get; set; }
    }
}