using NuciDAL.DataObjects;

namespace TransliterationAPI.Service.Entities
{
    public class CachedTransliteration : EntityBase
    {
        public string TransliteratedText { get; set; }
    }
}
