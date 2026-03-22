
using System.ComponentModel.DataAnnotations;
using NuciAPI.Requests;

namespace TransliterationAPI.API.Requests
{
    public class GetTransliterationRequest : NuciApiRequest
    {
        [StringLength(256, ErrorMessage = "The text cannot exceed 256 characters.")]
        public string Text { get; set; }

        public string Language { get; set; }
    }
}
