using System.Linq;

using Microsoft.AspNetCore.Mvc;
using NuciAPI.Controllers;
using TransliterationAPI.API.Requests;
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
                () => Language.GetAll().OrderBy(language => language.Code).ToList(),
                NuciApiAuthorisation.None);
    }
}
