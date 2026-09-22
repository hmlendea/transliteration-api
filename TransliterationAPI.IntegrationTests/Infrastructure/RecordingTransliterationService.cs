using System;
using System.Threading.Tasks;

using TransliterationAPI.Service;

namespace TransliterationAPI.IntegrationTests.Infrastructure
{
    internal sealed class RecordingTransliterationService : ITransliterationService
    {
        internal int InvocationCount { get; private set; }
        internal string? LastLanguageCode { get; private set; }
        internal string? LastText { get; private set; }
        internal Exception? ExceptionToThrow { get; set; }
        internal string? ResultToReturn { get; set; } = "Solaire of Astora";

        public Task<string> Transliterate(string text, string languageCode)
        {
            InvocationCount += 1;
            LastLanguageCode = languageCode;
            LastText = text;

            if (ExceptionToThrow is not null)
            {
                return Task.FromException<string>(ExceptionToThrow);
            }

            return Task.FromResult(ResultToReturn!);
        }
    }
}