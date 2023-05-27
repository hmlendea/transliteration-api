using System.Reflection;

namespace TransliterationAPI.Configuration
{
    public class CacheSettings
    {
        public string ApplicationVersion
        {
            get
            {
                if (string.IsNullOrWhiteSpace(applicationVersion))
                {
                    applicationVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString();
                }

                return applicationVersion;
            }
        }

        public string StoreLocation { get; set; }

        public bool Enabled { get; set; }

        string applicationVersion;

        public CacheSettings()
        {
            Enabled = true;
        }
    }
}
