using NuciAPI.Responses;
using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.API.Responses
{
    public class GetLanguagesResponse : NuciApiSuccessResponse
    {
        public Language[] Languages { get; set; }
    }
}
