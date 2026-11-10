using NuciAPI.Responses;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.API.Responses
{
    public sealed class LanguagesResponseContent : NuciApiResponseContent
    {
        public Language[] Languages { get; set; }

        public int Count => Languages.Length;
    }
}