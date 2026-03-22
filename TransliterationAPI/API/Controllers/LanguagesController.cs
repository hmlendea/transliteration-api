using System.Linq;

using Microsoft.AspNetCore.Mvc;
using NuciAPI.Controllers;
using TransliterationAPI.API.Requests;
using TransliterationAPI.API.Responses;
using TransliterationAPI.Configuration;
using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LanguagesController(SecuritySettings securitySettings) : NuciApiController
    {
        [HttpGet]
        public ActionResult Get()
            => ProcessRequest(
                new GetLanguagesRequest(),
                () =>
                {
                    GetLanguagesResponse response = new()
                    {
                        Languages = [.. Language.GetAll().OrderBy(language => language.Code)]
                    };

                    response.SignHMAC(securitySettings.HmacSigningKey);

                    return response;
                },
                NuciApiAuthorisation.None);
    }
}
