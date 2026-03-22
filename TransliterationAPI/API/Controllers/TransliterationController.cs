using Microsoft.AspNetCore.Mvc;
using NuciAPI.Controllers;
using TransliterationAPI.API.Requests;
using TransliterationAPI.API.Responses;
using TransliterationAPI.Service;

namespace TransliterationAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransliterationController(ITransliterationService transliterationService) : NuciApiController
    {
        [HttpGet]
        public ActionResult Get([FromQuery] GetTransliterationRequest request)
            => ProcessRequest(
                request,
                () => new GetTransliterationResponse
                {
                    Text = transliterationService.Transliterate(request.Text, request.Language).Result
                },
                NuciApiAuthorisation.None);
    }
}
