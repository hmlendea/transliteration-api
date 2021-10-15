using System;
using System.Web;

using Microsoft.AspNetCore.Mvc;

using TransliterationAPI.Service;

namespace TransliterationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransliterationController : ControllerBase
    {
        ITransliterationService transliterationService;

        public TransliterationController(ITransliterationService transliterationService)
        {
            this.transliterationService = transliterationService;
        }

        [HttpGet]
        public ActionResult Get(
            [FromQuery] string text,
            [FromQuery] string language)
        {
            try
            {
                string decodedText = HttpUtility.UrlDecode(text);
                string transliteratedText = transliterationService.Transliterate(text, language).Result; // TODO: Broken async
                return Ok(transliteratedText);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
