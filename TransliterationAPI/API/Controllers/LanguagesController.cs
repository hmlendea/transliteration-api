using System.Linq;

using Microsoft.AspNetCore.Mvc;
using NuciAPI.Controllers;
using TransliterationAPI.API.Requests;
using TransliterationAPI.API.Responses;
using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LanguagesController : NuciApiController
    {
        [HttpGet]
        public ActionResult Get()
            => ProcessRequest(
                new GetLanguagesRequest(),
                () => new GetLanguagesResponse
                {
                    Languages = [.. Language.GetAll().OrderBy(language => language.Code)]
                },
                NuciApiAuthorisation.None);
    }
}
