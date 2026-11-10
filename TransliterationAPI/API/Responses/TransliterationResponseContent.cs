using NuciAPI.Responses;

namespace TransliterationAPI.API.Responses
{
    public sealed class TransliterationResponseContent : NuciApiResponseContent
    {
        public string Text { get; set; }

        public override string ToString() => Text ?? string.Empty;
    }
}