using System.Linq;

using NuciAPI.Responses;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.API.Responses
{
    public sealed class LanguagesResponseContent : NuciApiResponseContent
    {
        public Language[] Languages { get; set; }

        public int Count => Languages.Length;

        public override string ToString()
            => string.Join("|", Languages.Select(language => $"{language.Code}:{language.Name}:{language.Transliterator}"));
    }
}