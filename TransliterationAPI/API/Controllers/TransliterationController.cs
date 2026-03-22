using System;
using System.Web;

using Microsoft.AspNetCore.Mvc;
using TransliterationAPI.API.Requests;
using TransliterationAPI.Service;

namespace TransliterationAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransliterationController(ITransliterationService transliterationService) : ControllerBase
    {
        [HttpGet]
        public ActionResult Get([FromQuery] GetTransliterationRequest request)
        {
            if (request.Text != null && request.Text.Length > 256)
            {
                return BadRequest("The text cannot exceed 256 characters");
            }

            try
            {
                string decodedText = HttpUtility.UrlDecode(request.Text);
                string transliteratedText = transliterationService.Transliterate(decodedText, request.Language).Result; // TODO: Broken async

                return Ok(transliteratedText);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
