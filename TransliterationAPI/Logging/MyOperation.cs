using NuciLog.Core;

namespace TransliterationAPI.Logging
{
    public sealed class MyOperation : Operation
    {
        MyOperation(string name) : base(name) { }

        public static Operation Transliteration => new MyOperation(nameof(Transliteration));

        public static Operation TransliteratorExecution => new MyOperation(nameof(TransliteratorExecution));
    }
}
