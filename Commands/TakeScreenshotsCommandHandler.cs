using System;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Screenshots.Commands
{
    public class TakeScreenshotsCommandHandler : AsyncRequestHandler<TakeScreenshotsCommand>
    {
        private readonly ILogger<TakeScreenshotsCommandHandler> _logger;
        private readonly ScreenshotsAsAServiceContext _dbContext;
 
        public TakeScreenshotsCommandHandler(
            ILogger<TakeScreenshotsCommandHandler> logger,
            ScreenshotsAsAServiceContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        protected override async Task Handle(
            TakeScreenshotsCommand command, CancellationToken cancellationToken) 
        {
            foreach(var url in command.Urls)
            {
                var screenshot = CaptureScreenshot(url);
                await InsertOrUpdateAsync(screenshot);
            }
            await _dbContext.SaveChangesAsync();
        }

        private Screenshot CaptureScreenshot(string url)
        {
            byte[] bytes;
            try 
            {
                bytes = WkhtmlDriver.Capture($" {url}", ImageFormat.Png);   
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to capture '{url}' screenshot!");
                throw;
            }
            return new Screenshot { Url = url, Bytes = bytes };
        }

        private async Task InsertOrUpdateAsync(Screenshot screenshot)
        {
            var existingScreenshot = await _dbContext.Screenshots.FindAsync(screenshot.Url);
            if (existingScreenshot == null)
            {
                await _dbContext.AddAsync(screenshot);
            }
            else
            {
               _dbContext.Entry(existingScreenshot).CurrentValues.SetValues(screenshot);
            }
        }
    }
}
