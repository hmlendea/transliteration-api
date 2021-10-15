using Microsoft.AspNetCore.Mvc;

namespace TransliterationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransliterationController : ControllerBase
    {
        [HttpGet]
        public string Get(
            [FromQuery] string language,
            [FromQuery] string text
        )
        {
            return "TEST! You sent: " + text + " in " + language;
        }
    }
}
