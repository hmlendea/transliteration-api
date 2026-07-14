using System.Reflection;

namespace TransliterationAPI.Configuration
{
    public sealed class CacheSettings
    {
        public string ApplicationVersion
            => Assembly.GetEntryAssembly()?.GetName().Version?.ToString();

        public string StoreLocation { get; set; }

        public bool Enabled { get; set; } = true;
    }
}
