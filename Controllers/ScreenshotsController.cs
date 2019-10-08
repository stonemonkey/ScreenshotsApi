using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Screenshots.Commands;
using Screenshots.Queries;

namespace Screenshots.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ScreenshotsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ScreenshotsController> _logger;

        public ScreenshotsController(IMediator mediator, ILogger<ScreenshotsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves taken screenshot of the HTML page foud at the specified URL.  
        /// </summary>
        /// <remarks>To take and save screenshot one needs first to POST the list of URLs.</remarks>
        /// <response code="200">OK.</response>
        /// <response code="400">The URL provided is invalid.</response>
        /// <response code="404">No screenshot found.</response>
        [HttpGet]
        public async Task<ActionResult> GetScreenshot([FromQuery] string url)
        {
            var query = new GetScreenshotQuery{ Url = url };
            if (!query.IsValid())
            {
                return BadRequest();
            }

            var result = await _mediator.Send(query);
            if (result is null)
            {
                return NotFound();
            }

            return File(result.Bytes, result.MimeType);
        }

        /// <summary>
        /// Takes screenshots of the HTML pages found at the specified URLs.
        /// </summary>
        /// <remarks>The request starts the operation and returns right away.</remarks>
        /// <response code="200">OK.</response>
        /// <response code="400">The URLs list provided is invalid (empty).</response>
        [HttpPost]
        public async Task<ActionResult> TakeScreenshots([FromBody] string[] urls)
        {
            var command = new TakeScreenshotsCommand{ Urls = urls };
            if (!command.IsValid())
            {
                return BadRequest();
            }

            await _mediator.Send(command);
            return Ok();
        }
    }
}
