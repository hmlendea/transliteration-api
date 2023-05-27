using System.Linq;

using Microsoft.AspNetCore.Mvc;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LanguagesController : ControllerBase
    {
        [HttpGet]
        public ActionResult Get()
            => Ok(Language.GetAll().OrderBy(language => language.Code));
    }
}
