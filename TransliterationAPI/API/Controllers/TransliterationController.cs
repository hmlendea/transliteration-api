using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using NuciAPI.Controllers;
using NuciAPI.Responses;

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
        public async Task<ActionResult> Get([FromQuery] GetTransliterationRequest request)
            => await ProcessRequest(
                request,
                async () =>
                {
                    NuciApiContentResponse<TransliterationResponseContent> response = new(
                        new()
                        {
                            Text = await transliterationService.Transliterate(request.Text, request.Language)
                        });
                    response.SignHMAC(securitySettings.HmacSigningKey);
                    return response;
                },
                NuciApiAuthorisation.None);
    }
}
