using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using CoreHtmlToImage;
using System.Collections.Generic;

namespace Screenshots.Commands
{
    public class TakeScreenshotsCommandHandler : IRequestHandler<TakeScreenshotsCommand, TakeScreenshotsResult>
    {
        private readonly ILogger<TakeScreenshotsCommandHandler> _logger;
        private readonly ScreenshotsAsAServiceContext _dbContext;

        private readonly HtmlConverter _htmlConverter;
 
        public TakeScreenshotsCommandHandler(
            ILogger<TakeScreenshotsCommandHandler> logger,
            ScreenshotsAsAServiceContext dbContext,
            HtmlConverter htmlConverter)
        {
            _logger = logger;
            _dbContext = dbContext;
            _htmlConverter = htmlConverter;
        }

        public async Task<TakeScreenshotsResult> Handle(
            TakeScreenshotsCommand command, CancellationToken cancellationToken)
        {
            // TODO: In order to scale the functionality for beeing able to capture large amounts of screenshots
            // here I would publish an event for a Capturing service, listening on a message queue, maybe using 
            // RabbitMQ or other message broker. 
            var errors = await Capture(command);

            return new TakeScreenshotsResult { FailedScreenshots = errors };
        }

        private async Task<TakeScreenshotError[]> Capture(TakeScreenshotsCommand command)
        {
            var errors = new List<TakeScreenshotError>();

            // TODO: In order to process multiple chunks of screenshots in parallel I would batch this and distribute
            // each batch to a separated job maybe using Hangfire.
            foreach (var url in command.Urls)
            {
                var screenshot = CaptureScreenshot(url);
                if (screenshot is null)
                    errors.Add(new TakeScreenshotError { Url = url });
                else
                    await InsertOrUpdateAsync(screenshot);
            }
            await _dbContext.SaveChangesAsync();

            return errors.ToArray();
        }

        private Screenshot CaptureScreenshot(string url)
        {
            Screenshot screenshot = null;
            try 
            {
                // I'm using here a small library (CoreHtmlToImage) for taking the screenshot. I'm not
                // happy with it because  it's not working in linux. This is the reason I'm using windows
                // containers to host the service. I tried couple of diffrent other libraries (e.g. IronPdf,
                // wkhtmltoimage) for this matter and neither one worked out of the box in linux containers. 
                // I spend some time trying fixing them unfortunately I run out of time. 
                screenshot = new Screenshot { Url = url, Bytes = _htmlConverter.FromUrl(url) };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to capture '{url}' screenshot!");
            }
            return screenshot;
        }

        private async Task InsertOrUpdateAsync(Screenshot screenshot)
        {
            var existingScreenshot = await _dbContext.Screenshots.FindAsync(screenshot.Url);
            if (existingScreenshot == null)
                await _dbContext.AddAsync(screenshot);
            else
               _dbContext.Entry(existingScreenshot).CurrentValues.SetValues(screenshot);
        }
    }
}
