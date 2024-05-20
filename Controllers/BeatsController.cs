using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace UnAssignmentWebApp.Controllers
{
    [ApiController]
    public class BeatsController : ControllerBase
    {
        private readonly Dictionary<int, string> config;
        private readonly bool isServerReady;

        public BeatsController(IOptions<CustomSection> cs)
        {
            config = cs.Value.Values;
            isServerReady = cs.Value.IsServerReady;
        }

        [HttpGet("/beatgenerator/{i}/{j}")]
        public IActionResult BeatGenerator(int i, int j)
        {
            if (i <= j)
            {
                return BadRequest("Parameter i must be superior to j");
            }

            List<string> beats = new List<string>();
            for (int num = j; num <= i; num++)
            {
                if (num % 12 == 0)
                {
                    beats.Add(config.TryGetValue(12, out string val) ? val : throw new KeyNotFoundException());
                }
                else if (num % 4 == 0)
                {
                    beats.Add(config.TryGetValue(4, out string val) ? val : throw new KeyNotFoundException());
                }
                else if (num % 3 == 0)
                {
                    beats.Add(config.TryGetValue(3, out string val) ? val : throw new KeyNotFoundException());
                }
                else
                {
                    beats.Add(config.TryGetValue(1, out string val) ? val : throw new KeyNotFoundException()); // default value
                }
            }

            return Ok(beats);
        }

        [HttpGet("/livez")]
        public IActionResult LiveCheck()
        {
            return Ok();
        }

        [HttpGet("/readyz")]
        public IActionResult ReadyCheck()
        {
            if (isServerReady)
            {
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
        }

        [HttpGet("/configure/{i}/{text}")]
        public IActionResult ConfigureBeat(int i, string text)
        {
            var textList = new List<string>()
            {
                "snare", "kick", "Hi-Hat", "Low Floor Tom", "cymbal", "Low-Mid Tom", "Bass Drum" 
            };

            if (i != 1 && i != 3 && i != 4 && i != 12)
            {
                return BadRequest("Invalid value for i");
            }

            if (!textList.Contains(text))
            {
                return BadRequest("Invalid value for text");
            }

            config[i] = text;
            return Ok();
        }

        [HttpGet("/reset")]
        public IActionResult ResetConfig()
        {
            config[1] = "Low Floor Tom";
            config[3] = "kick";
            config[4] = "snare";
            config[12] = "Hi-Hat";
            return Ok();
        }
    }
}
