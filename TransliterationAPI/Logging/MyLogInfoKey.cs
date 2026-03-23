using NuciLog.Core;

namespace TransliterationAPI.Logging
{
    public sealed class MyLogInfoKey : LogInfoKey
    {
        MyLogInfoKey(string name) : base(name) { }

        public static LogInfoKey Text => new MyLogInfoKey(nameof(Text));

        public static LogInfoKey TransliteratedText => new MyLogInfoKey(nameof(TransliteratedText));

        public static LogInfoKey LanguageCode => new MyLogInfoKey(nameof(LanguageCode));

        public static LogInfoKey LanguageName => new MyLogInfoKey(nameof(LanguageName));

        public static LogInfoKey Transliterator => new MyLogInfoKey(nameof(Transliterator));
    }
}
