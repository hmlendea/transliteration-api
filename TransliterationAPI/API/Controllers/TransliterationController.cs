using Microsoft.AspNetCore.Mvc;
using NuciAPI.Controllers;
using TransliterationAPI.API.Requests;
using TransliterationAPI.API.Responses;
using TransliterationAPI.Configuration;
using TransliterationAPI.Service;

namespace TransliterationAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransliterationController(
        ITransliterationService transliterationService,
        SecuritySettings securitySettings)
        : NuciApiController
    {
        [HttpGet]
        public ActionResult Get([FromQuery] GetTransliterationRequest request)
            => ProcessRequest(
                request,
                () =>
                {
                    GetTransliterationResponse response = new()
                    {
                        Text = transliterationService.Transliterate(request.Text, request.Language).Result
                    };

                    response.SignHMAC(securitySettings.HmacSigningKey);

                    return response;
                },
                NuciApiAuthorisation.None);
    }
}
